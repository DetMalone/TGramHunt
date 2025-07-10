using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using TGramHunt.Contract.Enums;

namespace TGramHunt.Contract.ViewModels.MainPage
{
    [BsonIgnoreExtraElements]
    public class ProductBase : ProductPicture
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public ProductCategory Category { get; set; }

        public DateTime DateOfCreation { get; set; }

        public int ImageCache { get; set; } = 1;

        public int Votes { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public ProductStatus? CurrentStatus { get; set; } = ProductStatus.Verified;

        [JsonIgnore]
        [BsonIgnore]
        public bool IsUserVoted { get; set; } = false;
    }
}