using DeviceInterface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HubActorFunction
{
    class Program
    {
        public static void Main(string[] args)
        {
            IMessageConsumer Consumer = new Consumer();
            Consumer.Listen(Process);
        }

        static async void Process(string message)
        {

            JObject Message = JObject.Parse(message);

            using (var client = await ConnectClient())
            {
                await DoClientWork(client, Message);
                Console.ReadKey();
            }

            Console.WriteLine("Message" + message);
        }

        private static async Task<IClusterClient> ConnectClient()
        {
       
            //IPEndPoint[] ips = new IPEndPoint[] {
            //    new IPEndPoint(IPAddress.Parse("104.46.55.39"), 30000),
            //};

            IClusterClient client;
            client = new ClientBuilder() //.UseStaticClustering(ips)
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "clu001";
                    options.ServiceId = "DeviceManagement";
                })
                
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client, JObject Message)
        {
            // example of calling grains from the initialized client
            var deviceSerialNumber = Message.GetValue("src").ToString();

            Console.WriteLine(deviceSerialNumber);
            var friend = client.GetGrain<IDevice>(deviceSerialNumber);
            Console.WriteLine("friend.GetPrimaryKeyLong: " + friend.GetPrimaryKeyString());

            var response = await friend.SetStatus("Good");
            Console.WriteLine("\n\n{0}\n\n", response);
            var response2 = await friend.AddAlert("Alert");
            Console.WriteLine("\n\n{0}\n\n", response);

        }
    }
}
