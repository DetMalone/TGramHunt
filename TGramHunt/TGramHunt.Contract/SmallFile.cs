using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TGramHunt.Contract
{
    public class SmallFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public byte[]? File { get; set; }
    }
}
