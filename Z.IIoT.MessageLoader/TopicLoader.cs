using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading.Tasks;

namespace Z.IIoT.MessageLoader
{
    class TopicLoader
    {
        void TopicMain(string[] args) {

            Publisher pub = new Publisher();
            pub.Go().Wait();
            Console.WriteLine("finished");
        }

        class Publisher
        {

            public MqttFactory Factory { get; set; }
            public IMqttClient MqttClient { get; set; }
            public IMqttClientOptions Options { get; set; }

            public Publisher() {

                // Create a new MQTT client.
                Factory = new MqttFactory();
                MqttClient = Factory.CreateMqttClient();
                var Options = new MqttClientOptionsBuilder()
                    .WithClientId("Client1")
                    .WithTcpServer("broker.hivemq.com", 1883) // Port is optional
                    .WithCredentials("bud", "%spencer%")
                    //.WithTls()
                    .WithCleanSession()
                    .Build();

                this.Options = Options;
            }

            public async Task Go()
            {
                await MqttClient.ConnectAsync(Options);


                MqttClient.Disconnected += async (s, e) =>
                {
                    Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try {
                        await MqttClient.ConnectAsync(Options);
                    }
                    catch {
                        Console.WriteLine("### RECONNECTING FAILED ###");
                    }
                };

                MqttClient.Connected += async (s, e) =>
                {
                    while (true)
                    {
                        var message = new MqttApplicationMessageBuilder()
                        .WithTopic("MyTopic")
                        .WithPayload("Hello World")
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                        await MqttClient.PublishAsync(message);
                    }
                };

            }
        }
    }
}