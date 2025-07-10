using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Models;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IUserVotesForProductsServices _userVotesForProductsServices;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public List<ProductsForPeriodViewModel> Products { get; set; } =
            new List<ProductsForPeriodViewModel>();

        public IndexModel(
            IUserVotesForProductsServices userVotesForProductsServices,
            IProductService productService,
            IUserService userService)
        {
            this._userVotesForProductsServices = userVotesForProductsServices;
            this._userService = userService;
            this._productService = productService;
        }

        public async Task<IActionResult> OnGet(ProductCategory category)
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            ProductCategory? filterCategory = category;
            if (!Enum.IsDefined(typeof(ProductCategory), category))
            {
                filterCategory = null;
            }

            var user = await _userService.GetIfAuthorized(User);
            Products = await this._productService.GetFirstProducts(
                null,
                verifiedOnly: true,
                filterCategory,
                currentUser: user?.Id);

            TempData["PreviosDate"] = Products.LastOrDefault()?.Date;
            ViewData[nameof(ProductCategory)] = category == 0 ? "all" : Enum.GetName(typeof(ProductCategory), category);

            return Page();
        }

        public async Task<IActionResult> OnGetLoadProducts(DateTime after, ProductCategory category)
        {
            List<ProductsForPeriodViewModel> oldProducts;
            ProductCategory? filterCategory = category;
            if (!Enum.IsDefined(typeof(ProductCategory), category))
            {
                filterCategory = null;
            }

            var user = await _userService.GetIfAuthorized(User);

            oldProducts = await this._productService.GetAfterProducts(
                after,
                null,
                verifiedOnly: true,
                filterCategory,
                currentUser: user?.Id);

            if (oldProducts.Any())
            {
                oldProducts.First().IsShowDate = oldProducts.FirstOrDefault().Date != (DateTime)TempData["PreviosDate"];
                TempData["PreviosDate"] = oldProducts.LastOrDefault().Date;
            }
            else
            {
                TempData["PreviosDate"] = null;
            }

            var partial = Partial("_ProductListPartial", oldProducts);
            partial.ViewData[nameof(ProductCategory)] = category == 0 ? "all" : Enum.GetName(typeof(ProductCategory), category);
            return partial;
        }

        public async Task<IActionResult> OnPostToVote(string productId, bool voteFlag)
        {
            // voteFlag = true if add vote else for remove
            var user = await _userService.GetIfAuthorized(HttpContext.User);

            if (user == null)
            {
                return Unauthorized();
            }

            var product = await this._productService.GetByTag(productId);

            if (product == null)
            {
                return new NotFoundObjectResult("Product not found");
            }

            if (product.DateOfCreation.Date != DateTime.UtcNow.Date)
            {
                return new BadRequestObjectResult("Product voting unavailable");
            }

            bool res;
            if (voteFlag)
            {
                res = await
                    this._userVotesForProductsServices
                    .AddVote(new UserVotesForProducts
                    {
                        ProductId = productId,
                        UserId = user.Id,
                        VotingDate = DateTime.UtcNow
                    });
            }
            else
            {
                res = await
                    this._userVotesForProductsServices
                    .RemoveVote(user.Id, productId);
            }

            if (!res)
            {
                return new BadRequestResult();
            }
            else
            {
                return new OkResult();
            }
        }
    }
}