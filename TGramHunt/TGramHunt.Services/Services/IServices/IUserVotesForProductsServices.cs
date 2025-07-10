using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Services.IServices
{
    public interface IUserVotesForProductsServices
    {
        Task<bool> AddVote(
            UserVotesForProducts userVotesForProducts);
        Task<bool> RemoveVote(
            Guid userId,
            string productId);
        Task DeleteUserVotes(Guid userId);
        Task DeleteProductVotes(string? productId);
        Task<UserVotesForProducts> Get(
            Guid userId,
            string productId);
        Task<long> GetCountForUser(Guid userId);
        Task<List<UserVotesForProducts>> GetAllVotesForUser(
            Guid userId,
            List<string>? prodTagsRange = null);
    }
}