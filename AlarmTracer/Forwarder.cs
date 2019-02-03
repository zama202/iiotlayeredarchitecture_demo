using DeviceInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlarmTracer
{
    class Forwarder : IDeviceObserver
    {
        public void ForwardTo(string message, string action)
        {
            Console.WriteLine(action); //Send to MES, Send to Edge 
            if (action.Equals("HW001"))
            {
                Console.WriteLine(action); //Send to MES, Send to Edge 
            }
        }
    }
}
