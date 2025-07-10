using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TGramHunt.Models
{
    public abstract class BasePageModel : PageModel
    {
        public int? Status { get; set; }

        public IActionResult TestHanler()
        {
            string name = HttpContext.Request.Query["handler"];
            if (!string.IsNullOrEmpty(name))
            {
                this.Status = 404;
                HttpContext.Response.StatusCode = 404;
                return Page();
            }

            return null;
        }

        public IActionResult OnPost()
        {
            return NotFound();
        }
    }
}
