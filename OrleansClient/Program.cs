using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime.Configuration;
using Orleans;
using System.Threading;
using HelloWorldInterfaces;

namespace OrleansClient
{
    class Program
    {
        static int Main(string[] args)
        {
            var config = ClientConfiguration.LocalhostSilo();
            try
            {
                InitializeWithRetries(config, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Orleans Client 初始化失败，异常:{ex}");

                Console.ReadLine();
                return 1;
            }

            DoClientWork().Wait();
            Console.WriteLine("请按任意键退出...");
            Console.ReadLine();
            return 0;
        }

        static void InitializeWithRetries(ClientConfiguration config, int attemptTimesWhenFailing)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);

                    Console.WriteLine("客户端与谷仓宿主连接成功");
                    break;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"第{attempt}次尝试连接Orleans Client失败");
                    if (attempt > attemptTimesWhenFailing)
                        throw ex;
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }

        static async Task DoClientWork()
        {
            var helloGrain = GrainClient.GrainFactory.GetGrain<IHello>(0);
            var response = await helloGrain.SayHello("你好，我来自客户端");

            Console.WriteLine("\n\n{0}\n\n", response);
        }
    }
}
