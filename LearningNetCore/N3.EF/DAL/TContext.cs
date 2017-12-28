using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using N3.EF.Entities;
using N3.EF.DTO;

namespace N3.EF.DAL
{
    public class TContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        //方法一
        //可以在注册TContext的时候就提供options，
        public TContext(DbContextOptions<TContext> options):base(options)
        {
            //如果有数据库存在，那么什么也不会发生。但是如果没有，那么就会创建一个数据库
            //这句代码不是必需的，如果加上，可能会与update-database命令冲突
            //Database.EnsureCreated();          


        }

        //方法二
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("the connection string");
        //    base.OnConfiguring(optionsBuilder);
        //}

        //Fluet Api参考 https://docs.microsoft.com/en-us/ef/core/modeling/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //不创建数据表
            modelBuilder.Ignore<CategoryDto>();
            modelBuilder.Ignore<ProductDto>();

            //modelBuilder.Entity<Product>().HasKey(x => x.ID);            
            //modelBuilder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(8,2)");

            //用这一句替代上面的两句
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());


        }

        //方法二
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("the connection string");
        //    base.OnConfiguring(optionsBuilder);
        //}

        //Fluet Api参考 https://docs.microsoft.com/en-us/ef/core/modeling/
        public DbSet<N3.EF.DTO.CategoryDto> CategoryDto { get; set; }

    }
}
