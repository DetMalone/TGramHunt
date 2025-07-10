using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class SmallFilesService : ISmallFilesService
    {
        private readonly ISmallFilesRepository _smallFilesRepository;

        public SmallFilesService(ISmallFilesRepository smallFilesRepository)
        {
            this._smallFilesRepository = smallFilesRepository;
        }

        public async Task Create(SmallFile smallFileDTO)
        {
            await this._smallFilesRepository.Create(smallFileDTO);
        }

        public async Task Delete(string? id)
        {
            await this._smallFilesRepository.Delete(id);
        }

        public async Task<SmallFile?> Get(string id)
        {
            return await this._smallFilesRepository.Get(id);
        }
    }
}
