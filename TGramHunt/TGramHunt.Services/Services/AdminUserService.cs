using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;

        public AdminUserService(UserManager<AdminUser> userManager,
            SignInManager<AdminUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;;
        }

        public async Task<AdminUser?> GetIfAuthorized(ClaimsPrincipal currentUser)
        {
            if (currentUser != null &&
                this._signInManager.IsSignedIn(currentUser))
            {
                var user = await this.Get(currentUser);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<AdminUser?> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return await _userManager.FindByIdAsync(id);
        }

        public async Task<AdminUser?> Get(ClaimsPrincipal currentUser)
        {
            if (currentUser == null)
            {
                return null;
            }

            return await _userManager.GetUserAsync(currentUser);
        }

        public async Task<AdminUser?> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> Login(AdminUser user, string password, bool staySignIn)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, staySignIn, false);
            return result.Succeeded;
        }

        public async Task<string?> Register(AdminUser user, string role, string? password = null)
        {
            IdentityResult result;
            if (string.IsNullOrEmpty(password))
            {
                result = await _userManager.CreateAsync(user);
            }
            else
            {
                result = await _userManager.CreateAsync(user, password);
            }

            if (!result.Succeeded)
            {
                return result?.Errors?.ToString() ?? string.Empty;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return "User Assign Role Error";
            }

            return null;
        }

        public async Task Remove(AdminUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> Update(AdminUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
