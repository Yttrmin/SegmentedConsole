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
            var Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var Random = new Random();
            while(true)
            {
                System.Threading.Thread.Sleep(250);
                Console.Write(Characters.ElementAt(Random.Next(Characters.Length)).ToString());
            }
        }
    }
}
