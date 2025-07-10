using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories.IRepositories;

namespace TGramHunt.Data.Repositories
{
    public class SmallFilesRepository : ISmallFilesRepository
    {
        private const string ID_FIELD = "_id";

        protected readonly IMongoCollection<SmallFile> dbCollection;

        public SmallFilesRepository(IMongoDBContext context)
        {
            this.dbCollection = context.GetCollection<SmallFile>(typeof(SmallFile).Name);
        }

        public async Task Create(SmallFile smallFileDTO)
        {
            if (smallFileDTO == null)
            {
                return;
            }

            await dbCollection.InsertOneAsync(smallFileDTO);
        }

        public async Task Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var filter = Builders<SmallFile>.Filter
                .Eq(ID_FIELD, ObjectId.Parse(id));
            await dbCollection.DeleteOneAsync(filter);
        }

        public async Task<SmallFile?> Get(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId idObj))
            {
                var filter = Builders<SmallFile>.Filter.Eq(ID_FIELD, idObj);

                return (await dbCollection.FindAsync<SmallFile>(filter)).FirstOrDefault();
            }

            return null;
        }
    }
}
