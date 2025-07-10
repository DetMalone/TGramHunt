using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.EditProfile;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Helpers;
using TGramHunt.Models;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Pages.Profile
{
    public class IndexModel : BasePageModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IProductVoteService _productVoteService;
        private readonly IMapper _mapper;
        private readonly IUserVotesForProductsServices _userVotesForProductsServices;

        public List<ProductsForPeriodViewModel> Products { get; set; } = new List<ProductsForPeriodViewModel>();

        public IndexModel(
            IUserService userService,
            IProductService productRepositoryForMainPage,
            IProductVoteService productVoteService,
            IMapper mapper,
            IUserVotesForProductsServices userVotesForProductsServices)
        {
            this._productService = productRepositoryForMainPage;
            this._userVotesForProductsServices = userVotesForProductsServices;
            this._productVoteService = productVoteService;
            this._userService = userService;
            this._mapper = mapper;
        }

        public ProfileViewModel Profile { get; set; }

        public async Task<IActionResult> OnGet(
            ProfileFilter? filter,
            ProductListSorting? sorting)
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            var user = await this._userService.GetIfAuthorized(User);
            if (user == null)
            {
                return Redirect("/");
            }

            this.FillViewData(ViewData, filter, sorting);
            if (filter.HasValue && filter.Value == ProfileFilter.upvotes)
            {
                this.Products = await this._productVoteService
                    .GetProductsVotedByUser(
                    user.Id,
                    sorting,
                    verifiedOnly: false);
            }
            else
            {
                this.Products = await this._productService
                    .GetFirstProducts(
                    sorting,
                    verifiedOnly: false,
                    null,
                    ownerId: user.Id,
                    currentUser: user.Id);
            }

            this.Profile = _mapper.Map<User, ProfileViewModel>(user);

            this.Profile.UpvotesCount = await
                this._userVotesForProductsServices
                    .GetCountForUser(user.Id);
            this.Profile.MyProductCount = await
                this._productService
                    .GetUserProductCount(user.Id);

            TempData["PreviosDate"] = Products.LastOrDefault()?.Date;

            return Page();
        }

        public async Task<IActionResult> OnGetLoadProducts(DateTime after,
            ProfileFilter? filter,
            ProductListSorting? sorting)
        {
            var user = await this._userService.GetIfAuthorized(User);
            if (user == null)
            {
                return Unauthorized();
            }

            List<ProductsForPeriodViewModel> oldProducts;
            if (filter.HasValue && filter.Value == ProfileFilter.upvotes)
            {
                oldProducts = await _productVoteService.GetAfterProductsVotedByUser(
                    after,
                    user.Id,
                    sorting,
                    verifiedOnly: false);
            }
            else
            {
                oldProducts = await _productService.GetAfterProducts(
                    after,
                    sorting,
                    verifiedOnly: false,
                    null,
                    ownerId: user.Id,
                    currentUser: user.Id);
            }

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
            var viewData = partial.ViewData;
            this.FillViewData(viewData, filter, sorting);

            return partial;
        }

        private void FillViewData(ViewDataDictionary viewData, ProfileFilter? filter, ProductListSorting? sorting)
        {
            viewData[nameof(ProfileFilter)] = filter ?? ProfileFilter.myProducts;
            viewData[nameof(ProductListSorting)] = sorting ?? ProductListSorting.mostPopular;

            var urlHelper = new CustomUrlHelper(this.Url);
            var sortList = new List<SelectListItem>
            {
                new SelectListItem {
                    Value = urlHelper.GenerateUrlParameters(filter, ProductListSorting.mostPopular),
                    Text = "Most popular",
                    Selected = ProductListSorting.mostPopular == sorting
                },
                new SelectListItem {
                    Value = urlHelper.GenerateUrlParameters(filter, ProductListSorting.newest),
                    Text = "Newest",
                    Selected = ProductListSorting.newest == sorting
                }
            };

            viewData["ProductListSortingSelect"] = sortList;
        }
    }
}