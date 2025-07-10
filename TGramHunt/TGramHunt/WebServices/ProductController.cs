using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using TGramHunt.Services.Services.IServices;
using TGramHunt.Validators;
using TGramHunt.ViewModels;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TGramHunt.Services.Helpers;
using Microsoft.Extensions.Options;
using TGramHunt.Configurations;
using TGramHunt.Services.Helpers.IHelpers;
using System;

namespace TGramHunt.WebServices
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ITgTagHelper _tgTagHelper;
        public ProductController(
            IProductService _productService,
            IUserService _userService,
            ITgTagHelper tgTagHelper)
        {
            this._productService = _productService;
            this._userService = _userService;
            _tgTagHelper = tgTagHelper;
        }

        [HttpPost("CheckByTag")]
        public async Task<IActionResult> CheckByTag([FromBody] string tag)
        {

            if ((await this._userService.GetIfAuthorized(User)) == null)
            {
                return Unauthorized();
            }

            var validator = new CreateProductViewModelValidator();
            var results = validator.Validate(new CreateProductViewModel() { Tag = tag },
                options => options.IncludeProperties("Tag"));

            results.AddToModelState(ModelState, null);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return new JsonResult(
                await _productService.IsProductExist(tag));
        }

        [HttpPost("CheckTagInfo")]
        public async Task<IActionResult> CheckTagInfo([FromBody] string tag)
        {
            if ((await this._userService.GetIfAuthorized(User)) == null)
            {
                return Unauthorized();
            }

            TagResult result = await _tgTagHelper.CheckTag(tag, false);
            
            return new JsonResult(!result.isEmpty);

        }
    }
}
