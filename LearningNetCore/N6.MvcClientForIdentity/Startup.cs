using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace N6.MvcClientForIdentity
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

            //关闭了JWT的Claim 类型映射，以保证它不会修改任何从Authorization Server返回的Claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //注册
            services.AddAuthentication(options=>
            {
                //验证用户的首选方式
                options.DefaultScheme = "Cookies";
                //当用户需要登陆的时候, 将使用的是OpenId Connect Scheme
                options.DefaultChallengeScheme = "oidc";

            })
            //添加了可以处理Cookie的处理器(handler)
            .AddCookie("Cookies")
            //让上面的handler来执行OpenId Connect 协议.
            .AddOpenIdConnect("oidc",options=>
            {
                options.SignInScheme = "Cookies";
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                //ClientId是Client的识别标志，需要在Authorization Server进行相应配置
                options.ClientId = "mvc_implicit";

                //既做Authentication又做Authorization. 也就是说既要id_token还要token本身
                //这样才能返回access token
                options.ResponseType = "id_token token";
                //表示要把从Authorization Server的Reponse中返回的token们持久化在cookie中
                options.SaveTokens = true;
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //在Startup的Configure方法配置中间件, 以确保每次请求都执行authentication
            //注意在管道配置的位置一定要在useMVC之前
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
