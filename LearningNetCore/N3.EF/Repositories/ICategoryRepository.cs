using N3.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N3.EF.Repositories
{
    /// <summary>
    /// IQueryable<T>还是IEnumerable<T>
    /// 观察一下
    /// public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate); 
    /// public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate); 
    /// 可以发现一个是Func委托，一个是Expression
    /// Func<>直接会被编译器编译成IL代码，
    /// Expression<>只是存储了一个表达式树，在运行期作处理，允许延迟处理，LINQ to SQL最终会将表达式树转为相应的SQL语句，然后在数据库中执行。
    /// </summary>
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetCategories();
        Category GetCategory(string cid,bool includeProducts);
        IEnumerable<Product> GetProductsOfCategory(string cid);
        Product GetProductOfCategory(string prpductId, string cid);
        Task<Category> GetCategoryAsync(string cid, bool includeProducts);
        Task<Product> GetProductOfCategoryAsync(string cid, string id);
        bool CategoryExist(string cid);
        bool ProductIdExist(string id);
        void AddProduct(Product product);
        void DeleteProduct(Product product);
        Task<bool> SaveAsync();
        
    }
}
