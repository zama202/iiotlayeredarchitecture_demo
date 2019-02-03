using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace Z.IIoT.HubConsumer
{
    interface IMessageProducer
    {
        void Produce(string message, string topic);
        void Produce(string message, Producer<Null, string> _producer, string topic);
    }
}
