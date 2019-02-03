using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace AlarmTracer
{
    class Consumer : IMessageConsumer
    {
        public void Listen(Action<string> message)
        {
            Console.WriteLine("Started");

            var config = new Dictionary<string, object>
            {
                {"group.id","cg-001-alarms" },
                {"bootstrap.servers", "kblvm02.westeurope.cloudapp.azure.com:9092" },
                { "enable.auto.commit", "false" }
            };

            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Subscribe("alarms");
                consumer.OnMessage += (_, msg) => {
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                    consumer.CommitAsync(msg);
                    message(msg.Value);
                };
                
                while (true)
                {
                    consumer.Poll(100);
                }
            }
        }

    }
}
