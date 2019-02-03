using System;
using System.Collections.Generic;
using System.Text;
using Cassandra;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HubConsumer;

namespace Z.IIoT.HubConsumer
{
    class Consumer : IMessageConsumer
    {
        public void ListenToEvent(Action<string, ISession, Writer> message, ISession session, Writer writer)
        {
            var config = new Dictionary<string, object>
            {
                {"group.id","cg-001-event" },
                {"bootstrap.servers", "z00060ipsmb001.westeurope.cloudapp.azure.com:9092" },
                { "enable.auto.commit", "false" }
            };

            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Subscribe("events");
                consumer.OnMessage += (_, msg) => {
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                    consumer.CommitAsync(msg);
                    message(msg.Value, session, writer);
                };
                
                while (true)
                {
                    consumer.Poll(100);
                }
            }
        }
        
        public void ListenToProcess(Action<string, ISession, Writer> message, ISession session, Writer writer)
        {
            var config = new Dictionary<string, object>
            {
                {"group.id","cg-002-process" },
                {"bootstrap.servers", "z00060ipsmb001.westeurope.cloudapp.azure.com:9092" },
                { "enable.auto.commit", "false" }
            };

            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Subscribe("processes");
                consumer.OnMessage += (_, msg) => {
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                    consumer.CommitAsync(msg);
                    message(msg.Value, session, writer);
                };

                while (true)
                {
                    consumer.Poll(100);
                }
            }
        }


        public void ListenToAlert(Action<string, ISession, Writer> message, ISession session, Writer writer)
        {
            var config = new Dictionary<string, object>
            {
                {"group.id","cg-003-alert" },
                {"bootstrap.servers", "z00060ipsmb001.westeurope.cloudapp.azure.com:9092" },
                { "enable.auto.commit", "false" }
            };

            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Subscribe("alerts");
                consumer.OnMessage += (_, msg) => {
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                    consumer.CommitAsync(msg);
                    message(msg.Value, session, writer);
                };

                while (true)
                {
                    consumer.Poll(100);
                }
            }
        }

    }
}
