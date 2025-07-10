using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories.IRepositories;

namespace TGramHunt.Data.Repositories
{
    public class UserVotesForProductsRepositories : IUserVotesForProductsRepositories
    {
        protected readonly IMongoDBContext _mongoContext;
        protected IMongoCollection<UserVotesForProducts> _dbCollection;
        protected readonly IProductRepository _productRepository;

        public UserVotesForProductsRepositories(
            IMongoDBContext context,
            IProductRepository productRepository)
        {
            this._mongoContext = context;
            this._dbCollection = _mongoContext
                .GetCollection<UserVotesForProducts>(typeof(UserVotesForProducts).Name);
            this._productRepository = productRepository;
        }

        public async Task<bool> AddVote(UserVotesForProducts userVotesForProducts)
        {
            if (userVotesForProducts == null)
            {
                return false;
            }

            try
            {
                var client = this._dbCollection.Database.Client;
                using var session = client.StartSession();
                var transactionOptions =
                    new TransactionOptions(
                        readPreference: ReadPreference.Primary,
                        readConcern: ReadConcern.Local,
                        writeConcern: WriteConcern.WMajority);

                await session
                    .WithTransactionAsync(async (s, ct) =>
                    {
                        await _dbCollection
                            .InsertOneAsync(
                                s,
                                userVotesForProducts,
                                null,
                                CancellationToken.None);

                        await this._productRepository
                        .IncrementVotes(
                            userVotesForProducts.ProductId,
                            s);

                        return string.Empty;
                    },
                transactionOptions,
                CancellationToken.None);
            }
            catch (MongoWriteException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveVote(
            Guid userId,
            string productId)
        {
            var client = this._dbCollection.Database.Client;
            using var session = client.StartSession();
            var transactionOptions =
                new TransactionOptions(
                    readPreference: ReadPreference.Primary,
                    readConcern: ReadConcern.Local,
                    writeConcern: WriteConcern.WMajority);

            var res = await session
                .WithTransactionAsync(async (s, ct) =>
                {
                    var filter = Builders<UserVotesForProducts>.Filter;
                    var def = filter.And(
                    filter.Eq(nameof(UserVotesForProducts.UserId), userId),
                    filter.Eq(nameof(UserVotesForProducts.ProductId), productId));

                    var deleteRes = await this._dbCollection.DeleteOneAsync(def, CancellationToken.None);

                    if (deleteRes.DeletedCount > 0)
                    {

                        await this._productRepository
                        .IncrementVotes(
                            productId,
                            s,
                            CancellationToken.None,
                            -1);

                        return string.Empty;
                    }

                    return null;
                },
            transactionOptions,
            CancellationToken.None);

            return res == string.Empty;
        }

        public async Task<UserVotesForProducts> Get(
            Guid userId,
            string productId)
        {
            return await _dbCollection
            .Find(x => x.UserId == userId &&
                x.ProductId == productId)
            .FirstOrDefaultAsync();
        }

        public async Task DeleteUserVotes(Guid userId)
        {
            var filter = Builders<UserVotesForProducts>.Filter
                .Eq(nameof(UserVotesForProducts.UserId), userId);

            await _dbCollection.DeleteManyAsync(filter);
        }

        public async Task DeleteProductVotes(string? productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            var filter = Builders<UserVotesForProducts>.Filter
                .Eq(nameof(UserVotesForProducts.ProductId), productId);
            await _dbCollection.DeleteManyAsync(filter);
        }

        public async Task<long> GetCount(string productId, IClientSessionHandle? clientSessionHandle)
        {
            return clientSessionHandle == null ?
                await _dbCollection
                .Find(x => x.ProductId == productId)
                .CountDocumentsAsync()
                : await _dbCollection
                .Find(clientSessionHandle,
                    x => x.ProductId == productId)
                .CountDocumentsAsync();
        }

        public async Task<long> GetCountForUser(Guid userId)
        {
            return await _dbCollection
                .Find(x => x.UserId == userId)
                .CountDocumentsAsync();
        }

        public async Task<List<UserVotesForProducts>> GetAllProductsIdsForUser(Guid userId, List<string>? prodTagsRange = null)
        {
            IFindFluent<UserVotesForProducts, UserVotesForProducts> query;
            if (prodTagsRange == null)
            {
                query = _dbCollection
                .Find(x => x.UserId == userId);
            }
            else
            {
                query = _dbCollection
                .Find(x => x.UserId == userId &&
                (prodTagsRange == null || (x.ProductId != null && prodTagsRange.Contains(x.ProductId))));
            }

            return await query
                .Project<UserVotesForProducts>(Builders<UserVotesForProducts>.Projection
                    .Include(nameof(UserVotesForProducts.ProductId))
                    .Include(nameof(UserVotesForProducts.UserId)))
                .ToListAsync();
        }
    }
}