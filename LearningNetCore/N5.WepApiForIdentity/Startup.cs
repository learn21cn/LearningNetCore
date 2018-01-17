using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace N5.WepApiForIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    //如果是生产环境, 一定要使用https，设置为true.
                    options.RequireHttpsMetadata = false;
                    //Authorization Server的地址
                    options.Authority = "http://localhost:5000";
                    //要和Authorization Server里面配置ApiResource的name一样
                    options.ApiName = "testapi";
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ApiForIdentity", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiForIdentity v1");
            });


            //把验证中间件添加到管道里, 这样每次请求就会调用验证服务了. 
            //一定要在UserMvc()之前调用
            //当在controller或者Action使用[Authorize]属性的时候, 这个中间件就会基于传递给api的Token来验证Authorization, 如果没有token或者token不正确, 这个中间件就会告诉我们这个请求是UnAuthorized(未授权的).
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
