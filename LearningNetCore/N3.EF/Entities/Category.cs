using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N3.EF.Entities
{
    public class Category
    {
        public string ID { get; set; }
        public string Name{ get; set; }
        public ICollection<Product> Products { get; set; }
        
    }
}
