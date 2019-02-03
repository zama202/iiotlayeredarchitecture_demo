using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Dynamic;

namespace DeviceSimulator
{
    public class Alphabet
    {
        
        public static JObject get(string[] v, int unixTimestamp)
        {
            //v[0] //name
            //v[1] //min
            //v[2] //max
            //v[3] //type
            //v[4] //unit   

            Int32.TryParse(v[1], out int min);
            Int32.TryParse(v[2], out int max);

            var rnd = new Random(DateTime.Now.Millisecond);
            int value = rnd.Next(min, max);

            JObject payload = new JObject(

            new JProperty("k", v[0]),
            new JProperty("v", value),
            new JProperty("ts", unixTimestamp)
            );

            return payload;

        }
    }
}
