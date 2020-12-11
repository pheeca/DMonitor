using DarazMonitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DarazMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            while (true)
            {
                var c = new DarazMonitorRunner();
                try
                {
                    c.startBrowser();
                    c.test();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(e));
                    Thread.Sleep(7000);
                }
                c.closeBrowser();
            }
        }
    }
}
