using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        //[FromBody],请求的body里面包含着方法需要的实体数据, 方法需要把这个数据Deserialize成Product.
        //这个特性的作用在于此
        [HttpPost("create")]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Price==1)
            {
                //复杂的验证，可以错误信息添加到ModelState里面
                //实际上是一个键值对
                ModelState.AddModelError("PriceError", "价格不能为1");
                //但是可以重复添加，键不变，值累加
                ModelState.AddModelError("Price", "价格不能为1");
                ModelState.AddModelError("Price", "价格不能为1");

                //更复杂的验证，推荐使用FluentValidation
                //https://github.com/JeremySkinner/FluentValidation
            }

            //验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            /*
              对于POST，建议的返回Status Code是201(Created),可以使用CreatedAtRoute这个内置的方法. 
              它可以返回一个带有地址Header的Response, 这个Location Header将会包含一个URI, 通过这个URI可以找到我们新创建的实体数据. 
              这里就是指之前写的GetProduct(int id)这个方法. 但是这个Action必须有一个路由的名字才可以引用它, 
              所以在GetProduct方法上的Route这个attribute里面加上Name="GetProduct", 然后在CreatedAtRoute方法第一个参数使用这个名字,
              CreatedAtRoute第二个参数就是对应着GetProduct的参数列表, 使用匿名类即可,
              最后一个参数是刚刚创建的数据实体. 
             */
            return CreatedAtRoute("GetProduct",new { id=newproduct.ID},newproduct);
        }
                
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var record = InitialData.Products.FirstOrDefault(x => x.ID == id);
            if (record == null)
            {
                return NotFound();
            }
            //注意这里不能直接将product赋值给record，而应每个属性逐一赋值
            record.Name=product.Name;
            record.Price = product.Price;
            record.Storehouses = product.Storehouses;

            
            return NoContent();

        }

        //可用于部分更新
        /*注意传递数据时的格式遵循以下形式，这一点与put是不同的
         * op 标识操作, replace 标识替换; path标识属性名, value为值.
         * 属性名默认用小写开头即可
        [
            {
                "op": "replace",
                "path": "/name",
                "value": "flower"
            },
	        {
			    "op": "replace",
                "path": "/price",
                "value": "1.5"		
	        }
         ]
         */
        [HttpPatch("{id}")]
        public IActionResult PatchProduct(int id,[FromBody] JsonPatchDocument<Product> patchDoc)
        {
            if (patchDoc==null)
            {
                return BadRequest();
            }

            var record = InitialData.Products.Find(x=>x.ID==id);
            if (record==null)
            {
                return NotFound();
            }

            var toPatch = new Product
            {
                Name = record.Name,
                Price=record.Price,
                Storehouses=record.Storehouses

            };
            
            //这句很关键
            patchDoc.ApplyTo(toPatch,ModelState);

            //添加验证，针对toPatch
            if (toPatch.Price <1)
            {
                ModelState.AddModelError("Price", "产品的价格不能小于1");
            }

            //验证，注意上面“产品的价格不能小于1”的验证不需要TryValidateModel就能起作用
            //但是，没有TryValidateModel这句话，Product类中的验证不起作用
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            record.Name = toPatch.Name;            
            record.Price = toPatch.Price;
            record.Storehouses = toPatch.Storehouses;
            
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var model = InitialData.Products.SingleOrDefault(x => x.ID == id);
            if (model == null)
            {
                return NotFound();
            }
            InitialData.Products.Remove(model);
            return NoContent();
        }

    }
}