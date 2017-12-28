using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using N3.EF.DAL;
using N3.EF.Entities;

namespace N3.EF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TContext _tcontext;

        public CategoryRepository(TContext tcontext)
        {
            _tcontext = tcontext;
        }

        public IEnumerable<Category> GetCategories()
        {
            //转化为List可立即执行
            return _tcontext.Categories.OrderBy(x => x.Name).ToList();
        }

        public Category GetCategory(string cid, bool includeProducts)
        {
            if (includeProducts)
            {
                return _tcontext.Categories
                    .Include(x => x.Products).FirstOrDefault(x => x.ID == cid);
            }
            else
            {
                return _tcontext.Categories.Find(cid);
            }
        }

        public async Task<Category> GetCategoryAsync(string cid, bool includeProducts)
        {
            if (includeProducts)
            {
                return await _tcontext.Categories
                    .Include(x => x.Products).FirstOrDefaultAsync(x => x.ID == cid);
            }
            else
            {
                return await _tcontext.Categories.FindAsync(cid);
            }
        }



        public Product GetProductOfCategory(string prpductId, string cid)
        {
            return _tcontext.Products.FirstOrDefault(x => x.ID == prpductId && x.CatogaryID == cid);

        }

        public async Task<Product> GetProductOfCategoryAsync(string cid, string prpductId)
        {
            return await _tcontext.Products.FirstOrDefaultAsync(x => x.ID == prpductId && x.CatogaryID == cid);

        }

        public IEnumerable<Product> GetProductsOfCategory(string cid)
        {
            return _tcontext.Products.Where(x => x.CatogaryID == cid).ToList();
        }

        public bool CategoryExist(string cid)
        {
            return _tcontext.Categories.Any(x => x.ID == cid);
        }

        public bool ProductIdExist(string id)
        {
            return _tcontext.Products.Any(x => x.ID == id);
        }

        public void AddProduct(Product product)
        {
            _tcontext.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            _tcontext.Products.Remove(product);
        }

        public async Task<bool> SaveAsync()
        {
             return await _tcontext.SaveChangesAsync() >= 0;
        }
    }
}
