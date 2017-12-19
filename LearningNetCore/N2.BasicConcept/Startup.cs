using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace N2.BasicConcept
{
    public class Startup
    {
        /*
        使用appSettings.json里面的值就需要使用实现了IConfiguration这个接口的对象。
        建议的做法是：在Startup.cs里面注入IConfiguration（这个时候通过CreateDefaultBuilder方法，它已经建立好了），
        然后把它赋给一个静态的property 
        */
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //以上代码是为了演示如何使用appSettings.json

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

#if DEBUG
            //AddTransient，AddScoped和AddSingleton，这些都表示service的生命周期
            services.AddTransient<ICustom, LocalService>();
#else
            //这句话的意思就是，当需要ICustom的一个实现的时候，Container就会提供一个CloudService的实例。
            services.AddTransient<ICustom, CloudService>();
#endif            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddProvider(new NLogLoggerProvider());
            //与上面的代码同样的作用，只是使用了一个扩展方法
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //正式使用时这段代码注释掉，否则影响api的访问
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            //使用这个中间件会使错误信息的提示更有好一些
            app.UseStatusCodePages();

            //使用mvc,因为在ConfigureServices中注册了，要使用api这个必须加上
            app.UseMvc();


        }
    }
}
