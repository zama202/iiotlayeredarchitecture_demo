using Cassandra;
using Newtonsoft.Json.Linq;
using System;
using Z.IIoT.HubConsumer;

namespace HubConsumer
{
    class Program
    {


        static void Main(string[] args)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("z00060ipsmb001.westeurope.cloudapp.azure.com").Build();
            ISession session_event = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();// Connect("ks_event");
            ISession session_process = cluster.Connect("ks_alert");
            ISession session_alert = cluster.Connect("ks_process");
            Writer writer = new Writer();
            Console.WriteLine("Hello World!");
            IMessageConsumer Consumer = new Consumer();
            Consumer.ListenToEvent(Process, session_event, writer);
            Consumer.ListenToEvent(Process, session_process, writer);
            Consumer.ListenToEvent(Process, session_alert, writer);
        }

        static void Process(string message, ISession session, Writer Writer) {
            JObject Message = JObject.Parse(message);
            Writer.Insert(session, Message);
            Console.WriteLine("Message" + message);
        }
    }
}
