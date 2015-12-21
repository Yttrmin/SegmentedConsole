using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SegmentedConsole.Console;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Poke();
            // Keep alive
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Write("A");
            }
        }
    }
}
