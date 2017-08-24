using Orleans;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectAsync().Wait();

            Console.Read();
        }

        static async Task ConnectAsync()
        {
            try
            {
                var config = ClientConfiguration.LocalhostSilo();
                var builder = new ClientBuilder().UseConfiguration(config);
                var client = builder.Build();
                await client.Connect();

                Console.WriteLine("客户端连接成功");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
