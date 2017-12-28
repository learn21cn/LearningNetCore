using N3.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N3.EF.DTO
{
    public class CategoryDto
    {
        public CategoryDto()
        {
            Products = new List<ProductDto>();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDto> Products { get; set; }

        public int ProductsCount => Products.Count;
    }

    
}
