using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace LearningNetCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //注册mvc
            services.AddMvc()
            //返回的json默认做了camel case的转化，即使用驼峰法命名，
            //添加前的结果示例 {"houseID":1,"houseName":"鲜花仓库1","quantity":1998} 
            //如果不想使用，添加下面的语句.AddJsonOptions
            //添加后的结果示例 {"HouseID":1,"HouseName":"鲜花仓库1","Quantity":1998}
            //注意只针对json
            .AddJsonOptions(options =>
            {
                if (options.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
                {
                    resolver.NamingStrategy = null;
                }
            })
            //如果需要允许返回xml格式，添加下面的.AddMvcOptions语句
            //这只是支持而已，如果需要真正的返回，在控制器中添加[Produces("application/xml")]特性，参见例子
            .AddMvcOptions(options =>
            {
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            })
            ;
            
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

            //使用mvc,因为在ConfigureServices中注册了
            app.UseMvc();

        }
    }
}
