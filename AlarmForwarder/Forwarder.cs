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

            Console.WriteLine("Forwarder!!");


            if (action.Equals("HW001"))
            {
                Console.WriteLine("raised " + action); //Send to MES, Send to Edge 
                Console.WriteLine(message); //Send to MES, Send to Edge 
            }
        }
    }
}
