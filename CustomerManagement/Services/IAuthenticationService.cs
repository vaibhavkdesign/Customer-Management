using CustomerManagement.Models;
using System.Threading.Tasks;


namespace CustomerManagement.Services
{
    public interface IAuthenticationService
    {
        Task RegisterUser(User user);
        Task<string> AuthenticateUser(UserCredentials userCredentials);
    }
}
