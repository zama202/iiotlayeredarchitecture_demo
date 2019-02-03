using System;
using System.Collections.Generic;
using System.Text;


namespace AlarmTracer
{
    interface IMessageConsumer
    {
        void Listen(Action<string> message);
    }
}
