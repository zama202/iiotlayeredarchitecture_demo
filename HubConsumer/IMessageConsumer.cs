using System;
using System.Collections.Generic;
using System.Text;
using Cassandra;
using HubConsumer;

namespace Z.IIoT.HubConsumer
{
    interface IMessageConsumer
    {
        void ListenToEvent(Action<string, ISession, Writer> message, ISession session, Writer writer);
        void ListenToProcess(Action<string, ISession, Writer> message, ISession session, Writer writer);
        void ListenToAlert(Action<string, ISession, Writer> message, ISession session, Writer writer);
    }
}
