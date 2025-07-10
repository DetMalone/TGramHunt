using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TGramHunt.Contract.Logging
{
    public class LoggingDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public LoggingType Type { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string? Message { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string? UserName { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string? StackTrace { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string? ExceptionType { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}