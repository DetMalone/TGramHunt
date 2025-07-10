using Microsoft.AspNetCore.Mvc;
using TGramHunt.Models;

namespace TGramHunt.Pages
{
    public class TermsOfServiceModel : BasePageModel
    {
        public IActionResult OnGet()
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            return Page();
        }
    }
}
