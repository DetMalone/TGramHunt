using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.LoginWidget;
using TGramHunt.Configurations;
using TGramHunt.Contract;
using TGramHunt.Services.Helpers.IHelpers;

namespace TGramHunt.Services.Helpers
{
    public class TelegramUserService : ITelegramUserService
    {
        public const string tgIdPattern = "tg{0}";

        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _userManager;

        public TelegramUserService(IOptions<AppSettings> appSettings,
            UserManager<User> userManager)
        {
            this._appSettings = appSettings.Value;
            this._userManager = userManager;
        }

        public string GetAuthTelegramWidget()
        {
            var botname = _appSettings.Telegram_Bot_Name;
            var proxyURL = _appSettings.Telegram_Ngrok_URL?.Trim('/');
            var tghook = LinkHelper.accauntHandlerLink;

#pragma warning disable S2696
            WidgetEmbedCodeGenerator.LoginWidgetJsVersion =
                _appSettings.Telegram_Bot_Version;
#pragma warning restore S2696

            return WidgetEmbedCodeGenerator
                .GenerateRedirectEmbedCode(
                    botname,
                    $"{proxyURL}{tghook}",
                    ButtonStyle.Large,
                    false,
                    true);
        }

        public async Task<User?> GetAndCreateIfNotExistsAuthTelegramWidget(Dictionary<string, string> query)
        {
            var token = _appSettings.Telegram_Bot_Token;
            var loginWidget = new LoginWidget(token);
            var auth = loginWidget.CheckAuthorization(query);

            // if the authorization was successful, create the user (if not exist) and sign in
            if (auth != Authorization.Valid)
            {
                return null;
            }

            var id = query["id"]; // must be present
            query.TryGetValue("first_name", out string? first_name);
            query.TryGetValue("last_name", out string? last_name);
            query.TryGetValue("photo_url", out string? photo_url);
            query.TryGetValue("username", out string? username);

            var tagId = string.Format(tgIdPattern, id);
            var user = await _userManager.FindByNameAsync(tagId);

            if (user == null)
            {
                user = new User()
                {
                    UserName = tagId,
                    Name = first_name ?? string.Empty,
                    Surname = last_name ?? string.Empty,
                    Picture = photo_url ?? string.Empty,
                    TelegramNativeId = long.Parse(id),
                    TelegramUserName = username ?? string.Empty,
                    RegistrationDate = System.DateTime.Now
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return null;
                }

                user = await _userManager.FindByNameAsync(tagId);
            }

            if (user.IsClosed)
            {
                return null;
            }

            return user;
        }
    }
}