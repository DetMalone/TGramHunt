using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TGramHunt.Contract.ViewModels.MainPage
{
    [BsonIgnoreExtraElements]
    public class ProductsForPeriodViewModel
    {
        public DateTime Date { get; set; }
        public bool IsShowDate { get; set; } = true;
        public List<ProductBase>? Products { get; set; }
    }
}
