using MongoDB.Driver;
using System.Linq;
using TGramHunt.Contract;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories.IRepositories;

namespace TGramHunt.Data.Repositories
{
    public class SystemSettingsRepository : ISystemSettingsRepository
    {
        protected readonly IMongoCollection<SystemSetting> dbCollection;

        private readonly string DB_NAME = "settings";

        public SystemSettingsRepository(IMongoDBContext context)
        {
            this.dbCollection = context.GetCollection<SystemSetting>(this.DB_NAME);
        }

        public SystemSetting GetSettings()
        {
            return this.dbCollection
                .Find(FilterDefinition<SystemSetting>.Empty)
                .FirstOrDefault();
        }
    }
}