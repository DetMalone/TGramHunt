using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TGramHunt.Contract
{
    public class SystemSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public bool IsLoggingEnabled { get; set; }
    }
}