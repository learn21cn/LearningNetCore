using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N3.EF.DAL;
using N3.EF.DTO;
using N3.EF.Entities;
using N3.EF.Repositories;

namespace N3.EF.Controllers
{
    [Produces("application/json")]
    [Route("api/Categories")]
    public class ProductsController : Controller
    {
        private readonly ICategoryRepository _iCategoryRepository;       

        public ProductsController(TContext context, ICategoryRepository iCategoryRepository)
        {
          
            _iCategoryRepository = iCategoryRepository;
        }

        // GET: api/Categories/1/Products
        [HttpGet("{cid}/Products")]
        public IEnumerable<ProductDto> GetProducts(string cid)
        {
            var ctegoryexit = _iCategoryRepository.CategoryExist(cid);
            if (!ctegoryexit)
            {
                return null;
            }
            var products = _iCategoryRepository.GetProductsOfCategory(cid);
            var results = products.Select(product => new ProductDto
            {
                ID = product.ID,
                Name = product.Name,
                CatogaryID = product.CatogaryID,
                Amount = product.Amount
                
            })
            .ToList();
            return results ;

        }

        // GET: api/Categories/5/Products/1
        [HttpGet("{cid}/Products/{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] string cid, [FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ctegoryexit = _iCategoryRepository.CategoryExist(cid);
            if (!ctegoryexit)
            {
                return NotFound();
            }

            var product = await _iCategoryRepository.GetProductOfCategoryAsync(cid,id);

            if (product == null)
            {
                return NotFound();
            }

            var result = new ProductDto
            {
                ID = product.ID,
                Name = product.Name,
                CatogaryID = product.CatogaryID,
                Amount =product.Amount
                
            };
            return Ok(result);

            
        }

        // PUT: api/Categories/putproduct/5
        [HttpPut("putproduct/{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] string id, [FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ID)
            {
                return BadRequest();
            }

            var product_init = _iCategoryRepository.GetProductOfCategory(id, product.CatogaryID);
            if (product_init == null)
            {
                return NotFound();
            }
            //使用了Mapper.Map的另一个重载方法，它有两个参数。这个方法会把第一个对象相应的值赋给第二个对象上。
            //这时候product的state就变成了modified了
            Mapper.Map(product, product_init);

            //然后保存
            if (!await _iCategoryRepository.SaveAsync())
            {
                return StatusCode(500, "更新时出现错误！");
            }           

            return NoContent();
        }

        // Patch: api/Categories/1/patchproduct/5
        //格式
        // [
        //	{
        //		"op": "replace",
        //		"path": "/amount",
        //		"value": 1000
        //    }
        //]
        [HttpPatch("{cid}/patchproduct/{id}")]
        public async Task<IActionResult> PatchProduct([FromRoute] string cid,[FromRoute] string id, [FromBody] JsonPatchDocument<ProductDto> patchDoc)
        {          

            if (patchDoc == null)
            {
                return BadRequest();
            }
            var product_init = _iCategoryRepository.GetProductOfCategory(id, cid);
            if (product_init == null)
            {
                return NotFound();
            }
            var toPatch = Mapper.Map<ProductDto>(product_init);
            patchDoc.ApplyTo(toPatch, ModelState);           
           
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }        
            
            Mapper.Map(toPatch, product_init);

            //然后保存
            if (!await _iCategoryRepository.SaveAsync())
            {
                return StatusCode(500, "更新时出现错误！");
            }

            return NoContent();
        }


        // POST: api/Categories/addproduct
        //可以单独创建一个ProductCreateDto类用来添加，此处使用了直接使用了ProductDto
        [HttpPost("addproduct")]
        public async Task<IActionResult> PostProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!_iCategoryRepository.CategoryExist(product.CatogaryID))
            {
                return StatusCode(500, "不存在这个分类！");
            }

            if (_iCategoryRepository.ProductIdExist(product.ID))
            {
                return StatusCode(500, "这个产品编号已经存在");
            }

            var newProduct = Mapper.Map<Product>(product);
             _iCategoryRepository.AddProduct(newProduct);

            if (!await _iCategoryRepository.SaveAsync())
            {
                return StatusCode(500, "保存时出现错误！");
            }

            return CreatedAtAction("GetProduct", new { cid=product.CatogaryID, id = product.ID, }, product);
        }

        // DELETE: api/Categories/1/deleteproduct/5
        [HttpDelete("{cid}/deleteproduct/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string cid,[FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product_init = _iCategoryRepository.GetProductOfCategory(id, cid);
            if (product_init == null)
            {
                return NotFound();
            }

            _iCategoryRepository.DeleteProduct(product_init);
            if (!await _iCategoryRepository.SaveAsync())
            {
                return StatusCode(500, "删除时出现错误！");
            }   
            return NoContent();
        }
       
    }
}