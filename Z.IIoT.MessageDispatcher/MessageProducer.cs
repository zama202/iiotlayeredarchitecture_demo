using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Z.IIoT.HubConsumer
{
    class MessageProducer : IMessageProducer
    {
        public Dictionary<string, object> ClusterConfig { get; set; }

        public void Produce(string message, string topic)
        {
            using (var producer = new Producer<Null, string>(ClusterConfig, null, new StringSerializer(Encoding.UTF8)))
            {
                producer.ProduceAsync(topic, null, message);
                producer.Flush(5);
            }
        }


        public void Produce(string message, Producer<Null, string> producer, string topic)
        {
            if (producer != null) {
                producer.ProduceAsync(topic, null, message);
                producer.Flush(5);
            }

           
        }
    }
}
