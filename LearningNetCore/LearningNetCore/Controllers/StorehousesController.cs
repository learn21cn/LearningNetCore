using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace N1.TheStart.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class StorehousesController : Controller
    {
        [HttpGet("{productID}/where")]
        public IActionResult GetMaterials(int productID)
        {
            var product = InitialData.Products.SingleOrDefault(x => x.ID == productID);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product.Storehouses);
        }

        //注意HttpGet特性中的参数与方法的参数必须对应起来
        [HttpGet("{productID}/where/{houseID}")]
        public IActionResult GetMaterial(int productID, int houseID)
        {
            var product = InitialData.Products.SingleOrDefault(x => x.ID == productID);
            if (product == null)
            {
                return NotFound();
            }
            var storehouse = product.Storehouses.SingleOrDefault(x => x.HouseID == houseID);
            if (storehouse == null)
            {
                return NotFound();
            }
            return Ok(storehouse);
        }
    }
}