using System;
using System.Linq;
using Console = SegmentedConsole.Console;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Poke();
            var Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var Random = new Random();
            Console.Write("HELLO WORLD!");
            // Keep alive
            while (true)
            {
                System.Threading.Thread.Sleep(250);
                Console.Write(Characters.ElementAt(Random.Next(Characters.Length)).ToString());
            }
        }
    }
}
