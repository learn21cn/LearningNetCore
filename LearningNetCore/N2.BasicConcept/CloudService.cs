using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N2.BasicConcept
{
    public class CloudService : ICustom
    {
        private readonly ILogger<CloudService> _logger;

        public CloudService(ILogger<CloudService> logger)
        {
            _logger = logger;
        }
        public void TestMessage(string subject, string msg)
        {
            _logger.LogInformation($"{subject}这是一个云环境！{msg}")  ;
        }
    }
}
