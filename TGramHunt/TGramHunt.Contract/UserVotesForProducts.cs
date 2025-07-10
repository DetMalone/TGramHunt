using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TGramHunt.Contract
{
    public class UserVotesForProducts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid UserId { get; set; }

        public string? ProductId { get; set; }

        public DateTime VotingDate { get; set; }
    }
}
