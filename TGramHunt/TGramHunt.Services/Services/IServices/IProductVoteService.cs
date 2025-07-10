using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;

namespace TGramHunt.Services.Services.IServices
{
    public interface IProductVoteService
    {
        Task<List<ProductsForPeriodViewModel>> GetProductsVotedByUser(Guid userId, ProductListSorting? _sorting, bool verifiedOnly);

        Task<List<ProductsForPeriodViewModel>> GetAfterProductsVotedByUser(DateTime after, Guid userId, ProductListSorting? _sorting, bool verifiedOnly);
    }
}
