using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N1.TheStart.Controllers
{
    /// <summary>
    /// 产品
    /// </summary>
    public class Product
    {
        public int ID { get; set; }      
        public string Name { get; set; }        
        public double Price { get; set; }
        public ICollection<Storehouse> Storehouses { get; set; }
    }
}
