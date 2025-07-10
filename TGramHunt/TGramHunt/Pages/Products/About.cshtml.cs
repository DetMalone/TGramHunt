using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Models;
using TGramHunt.Services.Helpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Pages.Products
{
    public class AboutModel : BasePageModel
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public Product Product { get; set; }

        public string TagHref { get; set; }

        public User ProductOwner { get; set; }

        public AboutModel(IProductService productService, IUserService userService)
        {
            this._productService = productService;
            this._userService = userService;
        }

        public async Task<IActionResult> OnGet(string tag)
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            ViewData["IdTest"] = tag;

            var user = await _userService.GetIfAuthorized(HttpContext.User);
            this.Product = await _productService.GetByTag(tag, user?.Id);
            if (this.Product == null)
            {
                return NotFound();
            }

            this.TagHref = LinkHelper.CreateLink(this.Product.Tag);
            this.ProductOwner = await this._userService.Get(this.Product.OwnerId.ToString());
            return Page();
        }
    }
}
