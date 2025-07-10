using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Helpers;
using TGramHunt.Models;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;
using TGramHunt.ViewModels;

namespace TGramHunt.Pages.Products
{
    [Authorize]
    public class NewModel : BasePageModel
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly LogOutHelper _logOutHelper;
        private readonly ITgTagHelper _tagHelper;
        private TagResult _productInfo;

        public NewModel(
            IProductService _productService,
            IMapper _mapper,
            LogOutHelper _logOutHelper,
            ITgTagHelper tgTagHelper)
        {
            this._productService = _productService;
            this._mapper = _mapper;
            this._logOutHelper = _logOutHelper;
            this._tagHelper= tgTagHelper;

        }

        public async Task<IActionResult> OnGet(string tag)
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            if (string.IsNullOrWhiteSpace(tag) ||
                tag[0] != '@')
            {
                return Redirect("/");
            }
            ViewData["tag"] = tag;

            _productInfo = await _tagHelper.CheckTag(tag, true);

            if (_productInfo.isEmpty)
            {
                return BadRequest("Product not found");
            }
            else
            {
                ViewData["description"] = _productInfo.Description;
                ViewData["name"] = _productInfo.Name;
                ViewData["category"] = _productInfo.Category;

                return Page();
            }
        }

        public async Task<IActionResult> OnPostCreateProduct([FromForm] CreateProductViewModel createProductVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("User not found");
            }

            var duplicateMessage = "Product already exist";
            if (await _productService.IsProductExist(createProductVM.Tag))
            {
                return BadRequest(duplicateMessage);
            }

            var product = _mapper.Map<CreateProductViewModel, Product>(createProductVM);

            var status = await _productService.CreateProduct(product, createProductVM.Cover, username);

            if (status == ProductCreationStatus.Duplicate)
            {
                return BadRequest(duplicateMessage);
            }
            else if (status == ProductCreationStatus.Unauthorized)
            {
                await this._logOutHelper
                    .Logout(HttpContext);
                return Unauthorized();
            }

            return StatusCode(200);
        }
    }
}
