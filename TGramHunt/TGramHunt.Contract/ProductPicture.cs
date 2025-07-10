using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TGramHunt.Contract
{
    [BsonIgnoreExtraElements]
    public class ProductPicture
    {
        [BsonId]
        public string? Tag { get; set; }

        public string? ImageIdx104 { get; set; }

        public string? ImageIdx56 { get; set; }

        public Guid OwnerId { get; set; }
    }
}