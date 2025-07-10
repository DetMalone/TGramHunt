using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TGramHunt.Contract.Enums;

namespace TGramHunt.ViewModels
{
    public class CreateProductViewModel
    {
        public string Tag { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ProductCategory Category { get; set; }

        public List<string> Makers { get; set; }

        public IFormFile Cover { get; set; }
    }
}
