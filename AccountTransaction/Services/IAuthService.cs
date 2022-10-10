
using AccountTransaction.Model;
using System.Threading.Tasks;

namespace Customer_API.Services
{
    public interface IAuthService
    {
        Task<User> Regiser(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExist(string username);
    }
}
