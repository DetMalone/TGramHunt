using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TGramHunt.Pages
{
    public class NotFoundModel : PageModel
    {
        public void OnGet()
        {
            Response.StatusCode = 404;
        }
    }
}