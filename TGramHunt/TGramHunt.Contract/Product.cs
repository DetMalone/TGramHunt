using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;

namespace TGramHunt.Contract
{
    [BsonIgnoreExtraElements]
    public class Product : ProductBase
    {
        [BsonRepresentation(BsonType.Int32)]
        public List<ProductSubject>? Subject { get; set; }

        public List<string>? Makers { get; set; }

        public DateTime? DateOfLastUpdate { get; set; }
    }
}