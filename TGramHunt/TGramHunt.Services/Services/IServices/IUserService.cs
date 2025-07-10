using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Services.IServices
{
    public interface IUserService
    {
        Task<User?> GetIfAuthorized(ClaimsPrincipal currentUser);
        Task Login(User user, bool staySignIn);

        Task<User?> Get(ClaimsPrincipal currentUser);

        Task<User?> Get(string id);

        Task<string?> Register(User user, string password, string role);

        Task<IdentityResult> Update(User user);

        Task Remove(User user);

        Task<string?> BaseRegister(User user, string password, string role);
    }
}
