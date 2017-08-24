using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using HelloWorldInterfaces;

namespace HelloWorldGrains
{
    public class HelloGrain : Grain, IHello
    {
        Task<string> IHello.SayHello(string greeting)
        {
            return Task.FromResult($"客户端说：{greeting}；服务器说 hello");
        }
    }
}
