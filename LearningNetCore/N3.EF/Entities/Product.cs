using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N3.EF.Entities
{
    public class Product
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string CatogaryID { get; set; }
        public decimal Amount { get; set; }
        public Category ProductCatogary { get; set; }

    }
}
