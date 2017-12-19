using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace N2.BasicConcept
{
    public class LocalService : ICustom
    {
        public void TestMessage(string subject, string msg)
        {
            Debug.WriteLine($"{subject}这是一个测试环境！{msg}");
        }
    }
}
