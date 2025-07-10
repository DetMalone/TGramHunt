using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class UserVotesForProductsServices : IUserVotesForProductsServices
    {
        private readonly IUserVotesForProductsRepositories _userVotesForProductsRepositories;

        public UserVotesForProductsServices(IUserVotesForProductsRepositories userVotesForProductsService)
        {
            this._userVotesForProductsRepositories = userVotesForProductsService;
        }

        public async Task<long> GetCountForUser(Guid userId)
        {
            return await _userVotesForProductsRepositories.GetCountForUser(userId);
        }

        public async Task<bool> AddVote(UserVotesForProducts userVotesForProducts)
        {
            return await _userVotesForProductsRepositories
                .AddVote(userVotesForProducts);
        }

        public async Task<bool> RemoveVote(
            Guid userId,
            string productId)
        {
            return await _userVotesForProductsRepositories
                    .RemoveVote(userId, productId);
        }

        public async Task DeleteUserVotes(Guid userId)
        {
            await _userVotesForProductsRepositories
                .DeleteUserVotes(userId);
        }

        public async Task DeleteProductVotes(string? productId)
        {
            await _userVotesForProductsRepositories
                .DeleteProductVotes(productId);
        }

        public async Task<UserVotesForProducts> Get(Guid userId, string productId)
        {
            return await _userVotesForProductsRepositories.Get(userId, productId);
        }

        public async Task<List<UserVotesForProducts>> GetAllVotesForUser(Guid userId, List<string>? prodTagsRange = null)
        {
            return await _userVotesForProductsRepositories
                .GetAllProductsIdsForUser(userId, prodTagsRange);
        }
    }
}
