using System;
using DeviceInterface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlarmTracer;

namespace AlarmForwarder
{
    class Program
    {

        static IClusterClient client = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            DoTheFuckingJob();
        }

        private static void DoTheFuckingJob() {

            if (null == client)
            {
                client = ConnectClient().GetAwaiter().GetResult();
            }
            DoClientWork(client).GetAwaiter().GetResult();
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

        private static async Task DoClientWork(IClusterClient client)
        {
            //var restclient = new RestClient("https://prod-34.westeurope.logic.azure.com:443/");
            //var request = new RestRequest("workflows/ae8551b470da49078596ab10638bfe58/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=weyC2_LkTjaX81VmnRzimnsyuYj1F3MUp3hDuQsiLrQ", Method.POST);

            //request.AddParameter("application/json", new JObject() , ParameterType.RequestBody);
            //request.RequestFormat = DataFormat.Json;


            //var res = restclient.Execute(request, Method.POST);
            //Console.WriteLine("response.StatusCode : " + res.StatusCode);


            // example of calling grains from the initialized client

            var friend = client.GetGrain<IDevice>("SN00001");

            int x = await friend.GetPendingOperationsCount();

            Console.WriteLine("friend.GetPrimaryKeyString: " + friend.GetPrimaryKeyString());
            Console.WriteLine("Count: " + x);


           Forwarder f = new Forwarder();




            //Create a reference for chat usable for subscribing to the observable grain.
            var obj = await client.CreateObjectReference<IDeviceObserver>(f);
            //Subscribe the instance to receive messages.

            //string response = await friend.Evaluate("{'pippo':'pluto'}");
            await friend.Subscribe(obj);
            //while (true) {
            //    await friend.Subscribe(obj);

            //    // send a publish
            //    await friend.Publish("aiuto!!!! Spegni la macchina, chiama il prete!", "HW001");

            //    Thread.Sleep(5000);
            //}
            
            

            //Console.WriteLine(response);
            Console.ReadLine();


        }
    }
}
