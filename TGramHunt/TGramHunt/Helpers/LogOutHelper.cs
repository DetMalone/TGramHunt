using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Helpers
{
    public class LogOutHelper
    {
        private readonly SignInManager<User> _signInManager;

        public LogOutHelper(SignInManager<User> _signInManager)
        {

            this._signInManager = _signInManager;
        }

        public async Task Logout(HttpContext context)
        {
            await this._signInManager.SignOutAsync();
            context.Session.Clear();
        }
    }
}