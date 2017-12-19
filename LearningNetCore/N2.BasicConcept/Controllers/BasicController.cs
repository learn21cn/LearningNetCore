using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace N2.BasicConcept.Controllers
{
    [Produces("application/json")]
    [Route("api/Basic")]
    public class BasicController : Controller
    {
        /*
        Ioc（Inversion of control）控制反转
        把为Controller选择某个依赖项（具有某个功能的Service）的具体实现类的这项工作委托给了外部的一个组件。
        Dependency Injection可以说是Ioc的一个特定的种类。
        DI模式是使用一个特定的对象（例如Container容器）来为目标类（例如某个Controller）进行初始化并提供其所需要的依赖项
        */

        private ILogger<BasicController> _logger;

        //使用内置的Logger，通过构造函数注入
        //如果不是用这种方式，可以通过请求 HttpContext.RequestServices.GetService(typeof(ILogger<BasicController>))获得，参见下文
        //public BasicController(ILogger<BasicController> logger)
        //{
        //    _logger = logger;
        //}


        //与上面同样的代码，用来测试外部日志组件
        //首先nuget安装Nlog，有很多，注意安装 NLog.Extensions.Logging 这一个
        //参考文档 https://github.com/NLog
        //在Startup.cs中的Configure方法中添加一个参数(ILoggerFactory loggerFactory)，
        //在方法中引入代码loggerFactory.AddProvider(new NLogLoggerProvider());
        //在项目更目录下添加并配置NLog.config文件 参见 https://github.com/NLog/NLog.Extensions.Logging
        //这样就用新的NLogLogger替代了内部的logger,其他代码可以不变

        //使用自定义服务
        private readonly ICustom _customService;
        public BasicController(ILogger<BasicController> logger,ICustom customService)
        {
            _logger = logger;
            _customService = customService;
        }

        [HttpGet("test")]
        public IActionResult BasicTest()
        {
            //通过请求获取内置Logger
            _logger = (ILogger<BasicController>) HttpContext.RequestServices.GetService(typeof(ILogger<BasicController>));

            //可在输出中进行查看
            _logger.LogInformation(DateTime.Now.ToString()+ "写日志了");

            try
            { throw new Exception(); }
            catch
            {
                _logger.LogCritical(DateTime.Now.ToString() + "测试");
            }

            return Ok("Hello World");
        }

        // 使用appSettings.json，先在Startup文件中进行相关配置
        //之间用冒号分开，表示它们的层次结构
        private readonly string area = Startup.Configuration["customSettings:area"];
        private readonly string server = Startup.Configuration["customSettings:server"];
        //可以新建一个appSettings.Development.json文件，试一下效果
        //可以发现它被作为appSettings.json的一个子文件显示出来
        //注意，Development是项目属性—调试 中的环境变量的名称，可以随意设置
        //这时项目实际起作用的会是appSettings.Development.json

        [HttpGet("nlog")]
        public IActionResult BasicNlog()
        {           
            //可在输出中进行查看
            _logger.LogInformation(DateTime.Now.ToString() + "写日志了");
            _customService.TestMessage("你好，", $"当前时间{DateTime.Now.ToString()}");
            _customService.TestMessage($"地区为{area}", $"服务器编号为{server}");
            return Ok("Hello World");
        }


    }
}