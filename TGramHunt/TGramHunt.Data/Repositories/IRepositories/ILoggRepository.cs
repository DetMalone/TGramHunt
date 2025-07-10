using System.Threading.Tasks;
using TGramHunt.Contract.Logging;

namespace TGramHunt.Data.Repositories.IRepositories
{
    public interface ILoggRepository
    {
        Task Log(LoggingDto dto);
    }
}