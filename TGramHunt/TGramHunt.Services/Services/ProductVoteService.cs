using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class ProductVoteService : IProductVoteService
    {
        private readonly IUserVotesForProductsServices _userVotesForProductsServices;
        private readonly IProductService _productService;

        public ProductVoteService(IUserVotesForProductsServices userVotesForProductsServices, IProductService productService)
        {
            this._userVotesForProductsServices = userVotesForProductsServices;
            this._productService = productService;
        }

        public async Task<List<ProductsForPeriodViewModel>> GetProductsVotedByUser(
            Guid userId,
            ProductListSorting? _sorting,
            bool verifiedOnly)
        {
            var productIds = (await this._userVotesForProductsServices.GetAllVotesForUser(userId) ?? new List<Contract.UserVotesForProducts>())
                .Where(x => !string.IsNullOrWhiteSpace(x.ProductId))
                .Select(x => x.ProductId);

            if (productIds == null || !productIds.Any())
            {
                return new List<ProductsForPeriodViewModel>();
            }

            return await _productService.GetFirstProducts(_sorting, verifiedOnly, null, null, productIds, currentUser: userId);
        }

        public async Task<List<ProductsForPeriodViewModel>> GetAfterProductsVotedByUser(DateTime after, Guid userId, ProductListSorting? _sorting, bool verifiedOnly)
        {
            var productIds = (await this._userVotesForProductsServices.GetAllVotesForUser(userId))
                .Where(x => !string.IsNullOrWhiteSpace(x.ProductId))
                .Select(x => x.ProductId);

            if (productIds == null || !productIds.Any())
            {
                return new List<ProductsForPeriodViewModel>();
            }

            return await _productService.GetAfterProducts(after, _sorting, verifiedOnly, null, null, productIds, currentUser: userId);
        }
    }
}
