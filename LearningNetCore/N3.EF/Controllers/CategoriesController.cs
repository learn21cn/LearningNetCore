using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class CategoriesController : Controller
    {        
        private readonly TContext _context;
        private readonly ICategoryRepository _iCategoryRepository;

        public CategoriesController(TContext context, ICategoryRepository iCategoryRepository)
        {
            _context = context;
            _iCategoryRepository = iCategoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public IEnumerable<CategoryWithoutProductsDto> GetCategories()
        {
            var categories = _iCategoryRepository.GetCategories();
            var results = new List<CategoryWithoutProductsDto>();
            foreach (var category in categories)
            {
                results.Add(new CategoryWithoutProductsDto
                {
                    ID = category.ID,
                    Name = category.Name,                  
                  
                });
            }
            return results;
        }

        //如果路由为api/Categories/5，参数有多个个，可以使用查询字符串
        //GET: api/Categories/5?includeProducts=true&参数2=1
        // GET: api/Categories/5/true
        [HttpGet("{id}/{includeProducts}")]
        public async Task<IActionResult> GetCategory([FromRoute] string id, [FromRoute] bool includeProducts=true)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _iCategoryRepository.GetCategoryAsync(id,includeProducts);

            if (category == null)
            {
                return NotFound();
            }


            if (includeProducts == true)
            {
                var result = new CategoryDto
                {
                    ID = category.ID,
                    Name = category.Name

                };
                //注意是逐个添加
                foreach (var product in category.Products)
                {
                    result.Products.Add(new ProductDto
                    {
                        ID = product.ID,
                        Name = product.Name,
                        Amount = product.Amount,
                        CatogaryID = product.CatogaryID
                    });
                }

                //这里重新构建了一个CategoryDto的result进行返回
                //是因为如果直接返回category，访问api会出现下面的错误
                //Error: Failure when receiving data from the peer
                //这个错误的原因在于Category中含有Products，每个Product中又包含了一个Category对象
                //所以使用CategoryDto代替Category，使用ProductDto代替Product
                return Ok(result);

            }
            else
            {
                var result = new CategoryWithoutProductsDto
                {
                    ID = category.ID,
                    Name = category.Name

                };
                return Ok(result);
            }          
           
        }

        //以下两个方法同上面的作用是一样的，只不过使用了AutoMapper
        //需要现在Startup.cs中进行配置
        // GET: api/Categories/Map
        [HttpGet("Map")]
        public IEnumerable<CategoryWithoutProductsDto> GetCategoriesMap()
        {
            var categories = _iCategoryRepository.GetCategories();

            //一句话就可以，不需要自己构建类型，可对比上面的方法
            var results = Mapper.Map<IEnumerable<CategoryWithoutProductsDto>>(categories);            
            return results;
        }
  
        // GET: api/Categories/Map/1/true
        [HttpGet("Map/{id}/{includeProducts}")]
        public async Task<IActionResult> GetCategoryMap([FromRoute] string id, [FromRoute] bool includeProducts = true)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _iCategoryRepository.GetCategoryAsync(id, includeProducts);

            if (category == null)
            {
                return NotFound();
            }

            if (includeProducts == true)
            {
                //使用Mapper.Map进行映射，<T>其中T是目标类型，可以是一个model也可以是一个集合，括号里面的参数是原对象们
                var result = Mapper.Map<CategoryDto>(category);   
                return Ok(result);
            }
            else
            {
                var result = Mapper.Map<CategoryWithoutProductsDto>(category);
                return Ok(result);
               
            }

        }
    }
}