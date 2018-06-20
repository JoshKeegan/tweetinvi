using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Logic.JsonConverters;
using Tweetinvi.Logic.Wrapper;
using Tweetinvi.Models;

namespace JsonEnumStringPerfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const string JSON = "{Type:\"message_create\"}";

            IJsonObjectConverter converter = new JsonObjectConverter(new JsonConvertWrapper());

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000000; i++)
            {
                SomeClass o = converter.DeserializeObject<SomeClass>(JSON);

                if (o.Type != EventType.MessageCreate)
                {
                    Console.WriteLine("bork!");
                }
            }

            sw.Stop();

            Console.WriteLine("{0}", sw.Elapsed);
        }
    }
}
