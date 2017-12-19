using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace N3.EF
{
    /// <summary>
    /// 与上一个项目使用NLog的方式不同
    /// 这里用nuget获取的是NLog.Web.AspNetCore，专门针对web
    /// 具体的使用说明参见 https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
    /// 这里直接在IWebHost BuildWebHost方法中添加UseNLog进行注册即可
    /// 当然也要添加配置文件
    /// 上一个项目使用的是 NLog.Extensions.Logging，针对.netcore
    /// 使用时不要两个都添加，避免冲突
    /// 这两个的异同参考 https://github.com/NLog     
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //这里注册nlog
                .UseNLog()
                .Build();
    }
}
