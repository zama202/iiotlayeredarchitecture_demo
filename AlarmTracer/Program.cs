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
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using RestSharp;

namespace AlarmTracer
{
    class Program
    {

        static IClusterClient client = null;

        public static void Main(string[] args)
        {
            IMessageConsumer Consumer = new Consumer();

            Consumer.Listen(Process);
        }

        static async void Process(string message)
        {

            JObject Message = JObject.Parse(message);

            if (null == client)
            {
                client = await ConnectClient();
            }
            else
            {
                await DoClientWork(client, Message);
            }


            //using (var client = await ConnectClient())
            //{
            //    await DoClientWork(client, Message);
            //    //Console.ReadKey();
            //    await client.Close();
            //    client.Dispose();
            //}
            
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
            //var restclient = new RestClient("https://prod-34.westeurope.logic.azure.com:443/");
            //var request = new RestRequest("workflows/ae8551b470da49078596ab10638bfe58/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=weyC2_LkTjaX81VmnRzimnsyuYj1F3MUp3hDuQsiLrQ", Method.POST);

            //request.AddParameter("application/json", new JObject() , ParameterType.RequestBody);
            //request.RequestFormat = DataFormat.Json;


            //var res = restclient.Execute(request, Method.POST);
            //Console.WriteLine("response.StatusCode : " + res.StatusCode);


            // example of calling grains from the initialized client
            var deviceSerialNumber = Message.GetValue("src").ToString();

            Console.WriteLine(deviceSerialNumber);
            var friend = client.GetGrain<IDevice>(deviceSerialNumber);
            Console.WriteLine("friend.GetPrimaryKeyString: " + friend.GetPrimaryKeyString());
            Message.Add("messageType", JToken.FromObject("warning"));
            // Ask to Evaluate Alarm
            Console.WriteLine("ALARM TRACER - Evaluate ALARM and increment PendingOperations");


            var response = await friend.Evaluate(Message.ToString());

//            Forwarder f = new Forwarder();
//            //Create a reference for chat usable for subscribing to the observable grain.
//            var obj = await client.CreateObjectReference<IDeviceObserver>(f);
//            //Subscribe the instance to receive messages.
//            await friend.Subscribe(obj);



            Console.WriteLine(response);
            
        }
    }
}
