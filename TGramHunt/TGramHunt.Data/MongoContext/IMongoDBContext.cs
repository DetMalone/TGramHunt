using MongoDB.Driver;

namespace TGramHunt.Data.MongoContext
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
