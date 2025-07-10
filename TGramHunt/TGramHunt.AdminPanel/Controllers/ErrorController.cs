using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TGramHunt.AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using TGramHunt.Helpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.AdminPanel.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}