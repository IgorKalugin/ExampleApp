using System.Threading.Tasks;
using Example.Model;
using ReactiveUI;

namespace Example.Services.AuthService
{
    public interface IAuthService : IReactiveObject
    {
        bool IsInitialized { get; }
        User CurrentUser { get; }
        
        Task<bool> Register(string userName, string email, string password);
        Task<bool> Login(string email, string password);
        Task<bool> Recover(string email);

        Task Logout();
    }
}