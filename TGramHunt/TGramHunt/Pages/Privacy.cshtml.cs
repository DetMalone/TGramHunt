using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TGramHunt.Models;

namespace TGramHunt.Pages
{
    public class PrivacyModel : BasePageModel
    {
        public PrivacyModel()
        {
        }

        public async Task<IActionResult> OnGet()
        {
            await Task.CompletedTask;
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            return Page();
        }
    }
}