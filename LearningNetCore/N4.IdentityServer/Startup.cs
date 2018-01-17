using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using N4.IdentityServer.Configuration;

namespace N4.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //配置
            services.AddIdentityServer()
                //.AddDeveloperSigningCredential()

                //password为生成pfx时输入的密码
                .AddSigningCredential(new X509Certificate2(@".\Certs\testapi.pfx", "password"))
                //需要配置Authorization Server来允许使用这些Identity Resources
                .AddInMemoryIdentityResources(TestConfigure.IdentityResources())
                .AddTestUsers(TestConfigure.Users().ToList())
                .AddInMemoryClients(TestConfigure.Clients())
                .AddInMemoryApiResources(TestConfigure.ApiResources());

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }
            //先用nuget安装IdentityServer
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
