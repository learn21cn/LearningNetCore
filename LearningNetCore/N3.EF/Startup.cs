using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N3.EF.DAL;
using N3.EF.DTO;
using N3.EF.Entities;
using N3.EF.Repositories;

namespace N3.EF
{
    public class Startup
    {

        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //使用AddDbContext这个扩展方法为TContext在Container中进行注册，它默认的生命周期使Scoped。
            //对应TContext中的方法二
            //services.AddDbContext<TContext>();

            //对应TContext中的方法一
            //var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=TestDB;Trusted_Connection=True";
            //可以将连接字符串配置在appSettings.json中，
            //也可以配置在项目的环境变量中（在项目属性中添加，自动写入launchSettings.json）
            //还可以配置在系统的环境变量中（生产环境一般使用这一种）
            //以上三种均可以使用下面的方式读取，其中connectionStrings:DbConnectionString为键的名称
            //如果在运行Add-Migration时提示找不到链接，需要重启项目或者计算机
            var connectionString = Configuration["connectionStrings:DbConnectionString"];
            services.AddDbContext<TContext>(o => o.UseSqlServer(connectionString));
            //注册
            services.AddScoped<ICategoryRepository,CategoryRepository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            //使用这个中间件会使错误信息的提示更有好一些
            app.UseStatusCodePages();

            //使用AutoMapper，需要先使用nuget进行安装
            //使用AutoMapper.Mapper.Initialize方法创建映射
            AutoMapper.Mapper.Initialize(cfg =>
            {
                //可以创建多个
                cfg.CreateMap<Category, CategoryWithoutProductsDto>();
                cfg.CreateMap<Category, CategoryDto>();
                //用于读
                cfg.CreateMap<Product,ProductDto>();
                //用于提交数据，从ProductDto到目标Product的映射，当然，可以ProductDto改写为ProductCreate以区分
                cfg.CreateMap<ProductDto, Product>();
            });

            //使用mvc,因为在ConfigureServices中注册了，要使用api这个必须加上
            app.UseMvc();
        }
    }
}
