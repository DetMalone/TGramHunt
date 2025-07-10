using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;

namespace TGramHunt.Services.Services.IServices
{
    public interface IProductService
    {
        Task<long> GetUserProductCount(Guid guid);

        Task<Product> GetByTag(string tag, Guid? ownerId = null);

        Task<ProductCreationStatus> CreateProduct(
            Product product,
            IFormFile cover,
            string owner);

        Task IncrementVotes(
            string? prodId,
            int count = 1);

        Task DeleteProduct(string? tag);

        Task<bool> IsProductExist(string tag);

        Task<IEnumerable<ProductPicture>> ListProductWithPicture(Guid userId);

        Task<List<ProductsForPeriodViewModel>> GetFirstProducts(ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null,
            Guid? currentUser = null);

        Task<List<ProductsForPeriodViewModel>> GetAfterProducts(DateTime after,
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null,
            Guid? currentUser = null);
    }
}
