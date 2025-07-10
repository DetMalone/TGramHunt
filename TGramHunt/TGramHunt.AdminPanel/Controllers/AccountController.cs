using Microsoft.AspNetCore.Mvc;
using TGramHunt.Services.Services.IServices;
using TGramHunt.AdminPanel.Views.Login;
using TGramHunt.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TGramHunt.Helpers;

namespace TGramHunt.AdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private IAdminUserService _userService;

        public AccountController(IAdminUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetByEmail(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.Email), "User Not Found");
                }
                else
                {
                    var isLogin = await _userService.Login(user, model.Password, false);
                    if (isLogin)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Password), "Login Failed: Invalid Password");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return Redirect("/");
        }
    }
}
