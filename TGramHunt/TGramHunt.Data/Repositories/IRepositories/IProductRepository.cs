using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;

namespace TGramHunt.Data.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<long> GetUserProductCount(Guid guid);

        Task Create(Product product);

        Task<Product> GetByTag(
            string tag,
            IClientSessionHandle? clientSessionHandle = null);

        Task IncrementVotes(
            string? prodId,
            IClientSessionHandle? session = null,
            CancellationToken? token = null,
            int count = 1);

        Task Delete(string? tag);

        Task<IEnumerable<ProductPicture>> ListProductWithPicture(Guid userId);

        Task<List<ProductsForPeriodViewModel>> GetFirstProducts(
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null);

        Task<List<ProductsForPeriodViewModel>> GetAfterProducts(
            DateTime after,
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null);
    }
}
