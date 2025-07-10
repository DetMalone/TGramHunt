using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.WebServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ITelegramUserService _telegramUserService;
        private readonly LogOutHelper _logOutHelper;
        private readonly IUserService _userService;

        public AuthController(
            SignInManager<User> signInManager,
            ITelegramUserService telegramUserService,
            LogOutHelper _logOutHelper,
            IUserService _userService)
        {
            this._signInManager = signInManager;
            this._telegramUserService = telegramUserService;
            this._logOutHelper = _logOutHelper;
            this._userService = _userService;
        }

        // /api/auth/logout
        [Authorize("AllowForClose")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await this._logOutHelper
                    .Logout(HttpContext);
            return Redirect("/");
        }

        [HttpPost("IsSignedIn")]
        [IgnoreAntiforgeryToken]
        public async Task<bool> IsSignedIn()
        {
            return (await this._userService.GetIfAuthorized(User)) != null;
        }

        // /api/auth/tgcallback
        [HttpGet("tgcallback")]
        public async Task<IActionResult> Tgcallback()
        {
            var queryCollection = Request.Query;
            var diction = new Dictionary<string, string>();
            foreach (var item in queryCollection)
            {
                diction.TryAdd(item.Key, item.Value);
            }

            var user = await this._telegramUserService
            .GetAndCreateIfNotExistsAuthTelegramWidget(diction);

            if (user == null)
            {
                await this._logOutHelper.Logout(HttpContext);
                return Redirect("/");
            }

            await _signInManager.SignInAsync(user, true);

            return Redirect("/");
        }
    }
}