using DarazMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            var c = new DarazMonitorRunner();
            try
            {
                c.startBrowser();
                c.test();
            }
            catch (Exception)
            {

            }
            c.closeBrowser();
        }
    }
}
