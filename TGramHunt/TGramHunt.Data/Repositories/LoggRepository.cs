using MongoDB.Driver;
using System.Threading.Tasks;
using TGramHunt.Contract.Logging;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories.IRepositories;

namespace TGramHunt.Data.Repositories
{
    public class LoggRepository : ILoggRepository
    {
        protected readonly IMongoCollection<LoggingDto> dbCollection;

        private readonly string DB_NAME = "logging";

        public LoggRepository(IMongoDBContext context)
        {
            this.dbCollection = context.GetCollection<LoggingDto>(this.DB_NAME);
        }

        public async Task Log(LoggingDto dto)
        {
            await this.dbCollection.InsertOneAsync(dto);
        }
    }
}