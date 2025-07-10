using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Data.Repositories.IRepositories
{
    public interface ISmallFilesRepository
    {
        Task Create(SmallFile smallFileDTO);
        Task<SmallFile?> Get(string id);
        Task Delete(string? id);
    }
}