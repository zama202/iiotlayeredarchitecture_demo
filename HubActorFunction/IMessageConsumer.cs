using System;
using System.Collections.Generic;
using System.Text;


namespace HubActorFunction
{
    interface IMessageConsumer
    {
        void Listen(Action<string> message);
    }
}
