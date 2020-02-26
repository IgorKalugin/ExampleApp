using System;
using System.Net;
using System.Threading.Tasks;
using Example.Services.LoggingService;
using Example.Services.SettingsService;
using RestSharp;
using RestSharp.Authenticators;
using Splat;
using ILogger = Example.Services.LoggingService.ILogger;

namespace Example.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly Lazy<ILogger> logger;
        private readonly Lazy<ISettingsService> settingsService;
        private readonly Lazy<IRestClient> restClient;
        
        public EmailService(IReadonlyDependencyResolver dr)
        {
            logger = this.GetLogger(dr);
            settingsService = new Lazy<ISettingsService>(() => dr.GetService<ISettingsService>());
            restClient = new Lazy<IRestClient>(() => new RestClient("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", settingsService.Value.MailgunApiKey)
            });
        }
        
        public async Task<bool> SendEmail(string subject, string body, string to, string attachmentPath)
        {
            logger.Value.Debug(nameof(SendEmail));
            
            var request = new RestRequest ();
            request.AddParameter ("domain", settingsService.Value.MailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", $"Mailgun <mailgun@{settingsService.Value.MailgunDomain}>");
            request.AddParameter ("to", to);
            request.AddParameter ("subject", subject);
            request.AddParameter ("text", body);
            request.AddFile("attachment", attachmentPath);
            request.Method = Method.POST;
            var response = await restClient.Value.ExecuteTaskAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.Value.Debug($"{nameof(SendEmail)} completed");
                return true;
            }
            
            logger.Value.Error($"{nameof(SendEmail)} error: {response.StatusCode}");
            return false;
        }
    }
}