using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N1.TheStart.Controllers
{
    public static class InitialData
    {
        public static List<Product> Products { get; set; } = new List<Product> {

             new Product
                {
                    ID = 1,
                    Name = "鲜花",                  
                    Price = 2.5,
                    Storehouses=new List<Storehouse>{
                        new Storehouse{ HouseID=1,HouseName="鲜花仓库1",Quantity=1998},
                        new Storehouse{ HouseID=2,HouseName="鲜花仓库2",Quantity=1119},
                    } 
                                        

                },
                new Product
                {
                    ID = 2,
                    Name = "蔬菜",                    
                    Price = 1.5,
                     Storehouses=new List<Storehouse>{
                        new Storehouse{ HouseID=1,HouseName="蔬菜仓库1",Quantity=125},
                        new Storehouse{ HouseID=2,HouseName="蔬菜仓库2",Quantity=215},
                    }
                }
        };
    }
}
