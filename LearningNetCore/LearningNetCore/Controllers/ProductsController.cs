using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace N1.TheStart.Controllers
{
    //默认 [Produces("application/json")]
    //如果需要返回xml,在Startup.cs的ConfigureServices方法中添加AddMvcOptions注册xml支持（参见Startup.cs）
    //之后改为[Produces("application/xml")]
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        //HttpGet这个特性不是必须的，添上这个特性，如果里面含有参数，则路由地址会改变
        //这是新的地址 http://localhost:5017/api/products/all
        //将参数作为了路由的一部分
        [HttpGet("test")]
        public JsonResult GetProductsTest()
        {
            return new JsonResult(InitialData.Products);
        }

        //地址是: "/api/product/{id}"，示例 http://localhost:5017/api/products/1
        //注意参数外面的大括号
        [Route("test/{id}")]
        public JsonResult GetProductTest(int id)
            => new JsonResult(InitialData.Products.FirstOrDefault(x => x.ID == id));


        [HttpGet("all")]
        public IActionResult GetProducts()
        {
            return Ok(InitialData.Products);
        }

        //使用 [Route("{id}")] 和 [HttpGet("{id}")]效果是一样的
        //赋值Name，方便后续使用
        [HttpGet("{id}",Name = "GetProduct")]
        public IActionResult GetProduct(int id)
        {
            var product = InitialData.Products.SingleOrDefault(x => x.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("create")]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var maxid = InitialData.Products.Max(x => x.ID);
            var newproduct = new Product()
            {
                ID = ++maxid,
                Name = product.Name,
                Price = product.Price,
                Storehouses = product.Storehouses
            };

            InitialData.Products.Add(newproduct);

            return CreatedAtRoute("GetProduct",new { id=newproduct.ID},newproduct);
        }






    }
}