using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Example.Data;
using Example.Model;
using Example.Services.LoggingService;
using Example.Services.SettingsService;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RestSharp;
using Splat;
using Xamarin.Essentials;
using ILogger = Example.Services.LoggingService.ILogger;

namespace Example.Services.AuthService
{
    public class AuthService : ReactiveObject, IAuthService
    {
        private const string ClientAuthTokenKey = "client_auth_token";
        private const string UserAuthTokenKey = "user_auth_token";
        
        private readonly Lazy<ILogger> logger;
        private readonly ISettingsService settingsService;
        private readonly IDatabase db;
        private readonly IScheduler scheduler;

        private readonly IRestClient restClient;

        private AuthToken clientAuthToken;    // The token is used to recover passwords on behalf of application when user is not logged in
        private AuthToken userAuthToken;    // The token is used to perform requests on behalf of user

        public AuthService(IReadonlyDependencyResolver dr)
        {
            // IMPORTANT: when you switch from debug to release or back clear SecureStorage!
            // SecureStorage.RemoveAll();
            
            logger = this.GetLogger(dr);

            settingsService = dr.GetService<ISettingsService>();
            restClient = new RestClient(settingsService.ServiceAddress);
            db = dr.GetService<IDatabase>();
            scheduler = dr.GetService<IScheduler>() ?? RxApp.MainThreadScheduler;
            
            // we should get current user synchronously because on it depends first view in bootstrapper (i's main page or register page)
            CurrentUser = db.GetCurrentUser();
        }

        [Reactive] public bool IsInitialized { get; private set; }
        [Reactive] public User CurrentUser { get; private set; }

        private async Task Initialize()
        { 
            var clientAuthTokenJson = await SecureStorage.GetAsync(ClientAuthTokenKey).ConfigureAwait(false);
            if (clientAuthTokenJson != null)
            {
                clientAuthToken = JsonConvert.DeserializeObject<AuthToken>(clientAuthTokenJson);
                logger.Value.Debug($"{nameof(clientAuthToken)} found");
            }

            if (CurrentUser == null)
            {
                IsInitialized = true;
                return;
            }

            var userAuthTokenJson = await SecureStorage.GetAsync(UserAuthTokenKey).ConfigureAwait(false);
            if (userAuthTokenJson != null)
            {
                userAuthToken = JsonConvert.DeserializeObject<AuthToken>(userAuthTokenJson);
                logger.Value.Debug($"{nameof(userAuthToken)} found");
            }

            IsInitialized = true;
        }
        
        public async Task<bool> Register(string userName, string email, string password)
        {
            return true;
            logger.Value.Debug($"{nameof(Register)} {nameof(userName)}={userName}, {nameof(email)}={email}");
            
            // ReSharper disable RedundantAnonymousTypePropertyName
            var contentObject = new { name = userName, email = email, password = password };
            // ReSharper restore RedundantAnonymousTypePropertyName
            
            var request = new RestRequest("account", Method.POST);
            request.AddJsonBody(contentObject);

            var response = await restClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.Value.Error($"Error received while registering user: code {response.StatusCode}");
                return false;
            }
            
            logger.Value.Debug("User registration completed successfully");
            return true;
        }

        public async Task<bool> Login(string email, string password)
        {
            return true;
            logger.Value.Debug($"{nameof(Login)} {nameof(email)}={email}");
            
            var request = new RestRequest("oauth/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", settingsService.UserClientId);
            request.AddParameter("client_secret", settingsService.UserClientSecret);
            request.AddParameter("username", email);
            request.AddParameter("password", password);
            request.AddParameter("scope", string.Empty);
            
            var response = await restClient.ExecutePostTaskAsync<TokenResponse>(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.Value.Error($"Error received while authorising user: code {response.StatusCode}");
                return false;
            }
            
            userAuthToken = ToAuthToken(response);
            var userAuthTokenJson = JsonConvert.SerializeObject(userAuthToken);
            await SecureStorage.SetAsync(UserAuthTokenKey, userAuthTokenJson).ConfigureAwait(false);

            // saving current user
            CurrentUser = await db.GetUserAsync(email).ConfigureAwait(false);
            if (CurrentUser == null)
            {
                CurrentUser = new User { Email = email, IsLoggedIn = true };
                await db.InsertAsync(CurrentUser).ConfigureAwait(false);
            }
            else
            {
                CurrentUser.IsLoggedIn = true;
                await db.UpdateAsync(CurrentUser).ConfigureAwait(false);
            }
            
            logger.Value.Debug("User authorisation completed successfully");
            return true;
        }

        public async Task<bool> Recover(string email)
        {
            logger.Value.Debug($"{nameof(Recover)} {nameof(email)}={email}");

            if (clientAuthToken == null)
            {
                var gotClientAuthToken = await GetClientAuthTokenAsync().ConfigureAwait(false);
                if (!gotClientAuthToken)
                {
                    return false;
                }
            }
            
            var request = new RestRequest("account/actions/reset_password", Method.POST);
            request.AddParameter("Authorization", $"Bearer {clientAuthToken.AccessToken}", ParameterType.HttpHeader);

            // ReSharper disable RedundantAnonymousTypePropertyName
            var contentObject = new { email = email };
            request.AddJsonBody(contentObject);
            // ReSharper restore RedundantAnonymousTypePropertyName
            
            var response = await restClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                logger.Value.Error($"Error received while restoring password: code {response.StatusCode}");
                return false;
            }
            
            logger.Value.Debug("Restoring password request sent successfully");
            return true;
        }

        public async Task Logout()
        {
            logger.Value.Debug(nameof(Logout));
            CurrentUser.IsLoggedIn = false;
            await db.UpdateAsync(CurrentUser).ConfigureAwait(false);
            CurrentUser = null;
            SecureStorage.Remove(UserAuthTokenKey);
        }

        private AuthToken ToAuthToken(IRestResponse<TokenResponse> response)
        {
            var expiresAt = scheduler.Now.AddSeconds(response.Data.expires_in);
            var token = new AuthToken
            {
                AccessToken = response.Data.access_token,
                RefreshToken = response.Data.refresh_token,
                ExpiresAt = expiresAt
            };
            return token;
        } 

        private async Task<bool> GetClientAuthTokenAsync()
        {
            logger.Value.Debug($"{nameof(GetClientAuthTokenAsync)}");
            var request = new RestRequest("oauth/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", settingsService.ApplicationClientId);
            request.AddParameter("client_secret", settingsService.ApplicationClientSecret);

            var response = await restClient.ExecutePostTaskAsync<TokenResponse>(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.Value.Error($"Error received while getting client auth token: code {response.StatusCode}");
                return false;
            }

            clientAuthToken = ToAuthToken(response);
            var clientAuthTokenJson = JsonConvert.SerializeObject(clientAuthToken);
            await SecureStorage.SetAsync(ClientAuthTokenKey, clientAuthTokenJson).ConfigureAwait(false);
            
            logger.Value.Debug($"{nameof(GetClientAuthTokenAsync)} completed successfully");
            return true;
        }
    }
}
