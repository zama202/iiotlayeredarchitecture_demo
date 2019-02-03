using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Z.IIoT.MessageLoader
{
    class QueueLoader
    {
        static void Main(string[] args)
        {
            //var target = "localhost";
            //var host = "127.0.0.1";
            //var queuename = "Default";

            var target = "SN00004";
            var host = "edgenode104.westeurope.cloudapp.azure.com";
            var queuename = "Alarms";
            

            if ((args.Length > 0) && (args.Length < 4)) {
                host = args[0];
                target = args[1];
                queuename = args[2];
            }

            Console.WriteLine(target);
            Console.WriteLine(queuename);

            var factory = new ConnectionFactory() { HostName = host, UserName = "node01", Password = "node01" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queuename, durable: false, exclusive: false, autoDelete: false, arguments: null);



                if (queuename.Equals("Events")) {
                    ///////////////////////////////////////////////////////////////////////
                    while (true)
                    {
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        List<JObject> sensors = new List<JObject>
                    {
                        Alphabet.get(new string[]{".DB_IOT.COOLING_WATER_INLET_TEMP_D","30","90","REAL","°C"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.COOLING_WATER_OUTLET_TEMP_D","10","25","REAL","°C"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.AIR_PRESSURE_ARS_D","0","20","REAL","bar"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.AIR_HIGH_PRESSURE_WEDGE_D","0","20","REAL","bar"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.AIR_PRESSURE_INJECTION_D","0","40","REAL","bar"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.AIR_PRESSURE_COMPRESSION_D","0","40","REAL","bar"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.ELECTRIC_VOLTAGE_PHASE_D","1","500","ARRAY[1..3]OFREAL","V"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.ELECTRIC_CURRENT_PHASE_D","2","4","ARRAY[1..3]OFREAL","A"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.ELECTRIC_POWER_D","300","600","REAL","KWh"},unixTimestamp),
                        Alphabet.get(new string[]{ ".DB_IOT.EXTRUDER.PET_HOPPER_TEMP_D","0","200","REAL","°C"},unixTimestamp),
                    };

                        foreach (JObject sensor in sensors)
                        {
                            Console.WriteLine(sensor.ToString());
                            channel.BasicPublish(exchange: "", routingKey: queuename, basicProperties: null, body: Encoding.UTF8.GetBytes(sensor.ToString()));
                        }
                        Thread.Sleep(100);
                    }
                    ///////////////////////////////////////////////////////////////////////
                }
                if (queuename.Equals("Alarms"))
                {
                    ///////////////////////////////////////////////////////////////////////
                    bool activeAlarm = false;

                    var rnd = new Random(DateTime.Now.Millisecond);
                    int value = rnd.Next(30000, 90000);


                    while (true)
                    {
                        Thread.Sleep(value);
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        activeAlarm = !activeAlarm;

                        JObject sensor = new JObject(

                            new JProperty("k", ".DB_IOT.EXTRUDER.ALARM"),
                            new JProperty("v", activeAlarm),
                            new JProperty("ts", unixTimestamp)
                        );

                        Console.WriteLine(sensor.ToString());
                        channel.BasicPublish(exchange: "", routingKey: queuename, basicProperties: null, body: Encoding.UTF8.GetBytes(sensor.ToString()));

                        value = rnd.Next(3000, 9000);

                        Thread.Sleep(100);
                    }

                    ///////////////////////////////////////////////////////////////////////
                }
                if (queuename.Equals("Products"))
                {
                    ///////////////////////////////////////////////////////////////////////
                    var rnd = new Random(DateTime.Now.Millisecond);
                    int value = rnd.Next(30000 + DateTime.Now.Millisecond, 90000 - DateTime.Now.Millisecond);


                    while (true)
                    {
                        Thread.Sleep(30000);
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        var sn = (value + "").PadLeft(7, '0');

                        JObject sensor = new JObject(
                            new JProperty("k", ".DB_IOT.PRODUCT.NUM"),
                            new JProperty("v", target + "000" + sn),
                            new JProperty("ts", unixTimestamp)
                        );

                        Console.WriteLine(sensor.ToString());
                        channel.BasicPublish(exchange: "", routingKey: queuename, basicProperties: null, body: Encoding.UTF8.GetBytes(sensor.ToString()));

                        value++;

                        Thread.Sleep(100);
                    }
                    ///////////////////////////////////////////////////////////////////////
                }

            }
        }
    }
}
