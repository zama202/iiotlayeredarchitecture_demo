using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Z.IIoT.HubConsumer
{
    class Program
    {

        static void Main(string[] args)
        {
            var target = "localhost";
            var host = "127.0.0.1";
            var queuename = "Default";
            var endpoint = "localhost:9092";
            var topic = "default";
            var bulk = 1;



            if ((args.Length > 0) && (args.Length < 6))
            {
                host = args[0];
                target = args[1];
                queuename = args[2];
                endpoint = args[3];
                topic = args[4];
                bulk = Convert.ToInt32(args[5]);

            }


            host = "edgenode104.westeurope.cloudapp.azure.com";
            target = "SN00004";
            queuename = "Alarms";
            endpoint = "z00060ipsmb001.westeurope.cloudapp.azure.com:9092";
            topic = "alarms";
            bulk = 1;

            //target = "S00001";
            //host = "edgenode101.westeurope.cloudapp.azure.com";
            //queuename = "Events";

            Console.WriteLine(target);
            Console.WriteLine(queuename);

            var factory = new ConnectionFactory() { HostName = host, UserName = "node01" , Password = "node01" };
            var ClusterConfig = new Dictionary<string, object> { { "bootstrap.servers", endpoint } };
            var messageProducer = new MessageProducer
            {
                ClusterConfig = ClusterConfig
            };
            var kafkaProducer = new Producer<Null, string>(ClusterConfig, null, new StringSerializer(Encoding.UTF8));


            bool IsConnected = false;
            while (!IsConnected) {

                try
                {
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queuename, durable: false, exclusive: false, autoDelete: false, arguments: null);

                        int counter = 0;

                        
                        JArray data = new JArray();

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            //Console.WriteLine("RECEIVED | counter = " + counter);
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            data.Add(JObject.Parse(message));

                            if (counter < bulk)
                            {
                                counter = counter + 1;
                            }
                        };

                        while (true) {
                            channel.BasicConsume(queue: queuename, autoAck: true, consumer: consumer);
                            //Console.WriteLine("while true : "  + counter);
                            if (counter == bulk)
                            {
                                JObject payload = new JObject();
                                payload.Add(new JProperty("src", target));
                                payload.Add(new JProperty("data", data));
                                payload.Add(new JProperty("dts", DateTime.Now));
                                data = new JArray();
                                counter = 0;
                                messageProducer.Produce(payload.ToString(), kafkaProducer, topic);
                                Console.WriteLine(" [x] Received {0}", payload.ToString());
                            }
                            Thread.Sleep(10);
                        }
                             
                    }
                }

                catch (Exception Ex)
                {
                    System.Threading.Thread.Sleep(5000);
                    IsConnected = false;
                    Console.WriteLine("Exception");

                    Console.WriteLine("isConnected? " + IsConnected);

                    Console.WriteLine(Ex);
                }
            }
            Console.WriteLine("Exiting");

        }
    }
}
