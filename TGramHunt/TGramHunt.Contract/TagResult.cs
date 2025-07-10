using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGramHunt.Contract.Enums;

namespace TGramHunt.Contract
{
    public class TagResult
    {
        public string? Tag { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public ProductCategory? Category { get; set; }

        public bool isEmpty { get; set; }
    }
}
