using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.Helpers;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories.IRepositories;

namespace TGramHunt.Repositories
{
    public class ProductRepository : IProductRepository
    {
        protected readonly IMongoCollection<Product> dbCollection;
        protected readonly IMongoCollection<ProductPicture> dbCollectionProduct;
        private const int PERIOD_IN_DAYS_FOR_INITIAL_LOAD = 1;
        private const int NUMBER_OF_ENTRIES_FOR_INFINITE_SCROLLING = 3;
        private const string ID_FIELD = "_id";

        public ProductRepository(
            IMongoDBContext context)
        {
            this.dbCollection = context
                .GetCollection<Product>(typeof(Product).Name);
            this.dbCollectionProduct = context
                .GetCollection<ProductPicture>(
                    typeof(ProductPicture).Name);
        }

        public async Task<long> GetUserProductCount(Guid guid)
        {
            return await dbCollection
                .Find(x => x.OwnerId == guid)
                .CountDocumentsAsync();
        }

        public async Task Create(Product product)
        {
            await dbCollection.InsertOneAsync(product);
        }

        public async Task Delete(string? tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }

            var filter = Builders<Product>.Filter.Eq(ID_FIELD, tag);
            await dbCollection.DeleteOneAsync(filter);
        }

        public async Task<Product> GetByTag(string tag, IClientSessionHandle? clientSessionHandle = null)
        {
            var filter = Builders<Product>.Filter.Eq(ID_FIELD, tag);
            return clientSessionHandle == null ?
                await dbCollection
                .Find(filter)
                .FirstOrDefaultAsync() :
                await dbCollection
                .Find(clientSessionHandle, filter)
                .FirstOrDefaultAsync();
        }

        public async Task IncrementVotes(
            string? prodId,
            IClientSessionHandle? session = null,
            CancellationToken? token = null,
            int count = 1)
        {
            if (string.IsNullOrWhiteSpace(prodId))
            {
                return;
            }

            var filter = Builders<Product>.Filter.Eq(ID_FIELD, prodId);
            var builder = Builders<Product>.Update;
            var update = builder.Inc(x => x.Votes, count);

            if (session == null)
            {
                await dbCollection.UpdateOneAsync(filter, update);
            }
            else
            {
                await dbCollection.UpdateOneAsync(session, filter, update, null, token ?? CancellationToken.None);
            }
        }

        public async Task<IEnumerable<ProductPicture>> ListProductWithPicture(Guid userId)
        {
            var fltr = Builders<Product>.Filter
                .Eq(nameof(Product.OwnerId),
                    userId);

            var projection = Builders<Product>.Projection
                .Include(u => u.Tag)
                .Include(u => u.OwnerId)
                .Include(u => u.ImageIdx104)
                .Include(u => u.ImageIdx56);

            return await dbCollection
                .Find(fltr)
                .Project<ProductPicture>(projection)
                .ToListAsync();
        }

        public async Task<List<ProductsForPeriodViewModel>> GetFirstProducts(
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null)
        {
            var comparer = new ProductComparer(_sorting);

            return await this.GetProducts(
                GetFilterForProducts(DateTime.Now, verifiedOnly, productCategory, ownerId, productIds),
                comparer);
        }

        public async Task<List<ProductsForPeriodViewModel>> GetAfterProducts(
            DateTime after,
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null)
        {
            var comparer = new ProductComparer(_sorting);
            var products = await dbCollection
                .Find(GetFilterForProducts(after, verifiedOnly, productCategory, ownerId, productIds))
                .SortByDescending(x => x.DateOfCreation)
                .ThenByDescending(x => x.Votes)
                .Limit(NUMBER_OF_ENTRIES_FOR_INFINITE_SCROLLING)
                .ToListAsync();
            return ConvertToProductForMainPageViewModel(comparer, products);
        }

        private static List<ProductsForPeriodViewModel> ConvertToProductForMainPageViewModel(
            ProductComparer comparer,
            List<Product> products)
        {
            return products
                .GroupBy(x => x.DateOfCreation.Date)
                .Select(x => new ProductsForPeriodViewModel
                {
                    Date = x.Key,
                    Products = x
                        .Select(y => new ProductBase
                        {
                            Tag = y.Tag,
                            Category = y.Category,
                            DateOfCreation = y.DateOfCreation,
                            Description = y.Description,
                            Name = y.Name,
                            Votes = y.Votes,
                            ImageIdx56 = y.ImageIdx56,
                            ImageIdx104 = y.ImageIdx104,
                            CurrentStatus = y.CurrentStatus
                        })
                        .OrderBy(x => x, comparer)
                        .ToList()
                })
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        private async Task<List<ProductsForPeriodViewModel>> GetProducts(
            FilterDefinition<Product> filter,
            ProductComparer comparer)
        {
            return (await dbCollection
                    .Aggregate()
                    .Match(filter)
                    .Group(BsonDocument.Parse("{ _id: " +
                        "{" +
                            "year: { $year: '$DateOfCreation' }, " +
                            "month: { $month: '$DateOfCreation' }," +
                            "day: { $dayOfMonth: '$DateOfCreation' }" +
                        "}" +
                        "Products: { '$push': '$$ROOT'}}"))
                    .SortByDescending(x => x[ID_FIELD])
                    .Limit(PERIOD_IN_DAYS_FOR_INITIAL_LOAD)
                    .Project<ProductsForPeriodViewModel>(BsonDocument.Parse("{ " +
                        "_id: 0, " +
                        "Date: { $dateFromParts: { 'year': '$_id.year', 'month': '$_id.month', 'day': '$_id.day' } }, " +
                        "Products: '$Products' }"
                    ))
                    .ToListAsync())
                    .ConvertAll(x => new ProductsForPeriodViewModel
                    {
                        Date = x.Date,
                        Products = x.Products == null ?
                            new List<ProductBase>() :
                            x.Products
                                .OrderBy(x => x, comparer).ToList()
                    });
        }

        private static FilterDefinition<Product> GetFilterForProducts(
            DateTime startPeriod,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null)
        {
            var filter = Builders<Product>.Filter;
            var builder = filter.Lt(nameof(Product.DateOfCreation), startPeriod);

            if (verifiedOnly)
            {
                builder &= filter.Eq(nameof(Product.CurrentStatus), ProductStatus.Verified.ToString());
            }

            var productIdsFilter = filter.Empty;
            if (productIds != null && productIds.Any())
            {
                var idsArray = productIds.ToArray();
                productIdsFilter = filter.Eq(ID_FIELD, idsArray[0]);
                for (var i = 1; i < idsArray.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(idsArray[i]))
                    {
                        continue;
                    }

                    productIdsFilter |= filter.Eq(ID_FIELD, idsArray[i]);
                }
            }

            if (productCategory != null)
            {
                builder &= Builders<Product>.Filter.Eq(nameof(Product.Category), productCategory);
            }

            if (ownerId != null)
            {
                builder &= Builders<Product>.Filter.Eq(nameof(Product.OwnerId), ownerId);
            }

            return productIdsFilter & builder;
        }
    }
}