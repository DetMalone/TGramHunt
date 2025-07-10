using Microsoft.AspNetCore.Mvc;
using TGramHunt.Models;

namespace TGramHunt.Pages
{
    public class EmptyModel : BasePageModel
    {
        public EmptyModel() { }

        // this page is used for telegram component
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
