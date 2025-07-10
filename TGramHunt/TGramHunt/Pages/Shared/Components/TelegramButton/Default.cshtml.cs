using Microsoft.AspNetCore.Mvc;
using System;
using TGramHunt.Services.Helpers.IHelpers;

namespace TGramHunt.Pages.Shared.Components.TelegramButton
{
    public class TelegramButton : ViewComponent
    {
        private readonly ITelegramUserService _telegramUserService;

        public string WidgetEmbedCode { get; set; }

        public string ContainerGuid { get; set; }

        public TelegramButton(
            ITelegramUserService telegramUserService)
        {
            this._telegramUserService = telegramUserService;
        }

        public void OnGet()
        {
            // do nothing
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.WidgetEmbedCode = this._telegramUserService.GetAuthTelegramWidget();

            ViewBag.ContainerGuid = Guid.NewGuid().ToString();

            return View();
        }
    }
}