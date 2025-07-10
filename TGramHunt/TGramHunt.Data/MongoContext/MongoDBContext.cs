using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDbGenericRepository.Utils;

namespace TGramHunt.Data.MongoContext
{
    public class MongoDBContext : IMongoDBContext
    {
        private IMongoDatabase Db { get; set; }

        private MongoClient MongoClient { get; set; }

        public IGridFSBucket GridFS { get; private set; }

        public MongoDBContext(IOptions<Mongosettings> configuration)
        {
            this.MongoClient = new MongoClient(configuration.Value.Connection);
            this.Db = this.MongoClient.GetDatabase(configuration.Value.DatabaseName);
            this.GridFS = new GridFSBucket(Db);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return Db.GetCollection<T>(name.Pluralize().Camelize());
        }
    }
}