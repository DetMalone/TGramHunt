using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Data.Repositories.IRepositories
{
    public interface IUserVotesForProductsRepositories
    {
        Task<bool> AddVote(UserVotesForProducts userVotesForProducts);

        Task<bool> RemoveVote(
            Guid userId,
            string productId);

        Task<UserVotesForProducts> Get(Guid userId, string productId);

        Task<long> GetCount(string productId, IClientSessionHandle? clientSessionHandle);

        Task<long> GetCountForUser(Guid userId);

        Task<List<UserVotesForProducts>> GetAllProductsIdsForUser(Guid userId, List<string>? prodTagsRange = null);

        Task DeleteUserVotes(Guid userId);

        Task DeleteProductVotes(string? productId);
    }
}
