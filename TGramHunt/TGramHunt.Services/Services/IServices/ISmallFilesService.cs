using System.Threading.Tasks;
using TGramHunt.Contract;

namespace TGramHunt.Services.Services.IServices
{
    public interface ISmallFilesService
    {
        Task Create(SmallFile smallFileDTO);
        Task<SmallFile?> Get(string id);
        Task Delete(string? id);
    }
}