using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Services.IServices
{
    public interface IAdminUserService
    {
        Task<bool> Login(AdminUser user, string password, bool staySignIn);

        Task Logout();

        Task<AdminUser?> GetIfAuthorized(ClaimsPrincipal currentUser);

        Task<AdminUser?> Get(ClaimsPrincipal currentUser);

        Task<AdminUser?> Get(string id);

        Task<AdminUser?> GetByEmail(string email);

        Task<string?> Register(AdminUser user, string role, string? password);

        Task<IdentityResult> Update(AdminUser user);

        Task Remove(AdminUser user);
    }
}
