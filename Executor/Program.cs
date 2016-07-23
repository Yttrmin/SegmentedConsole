using System;
using System.Linq;
using Console = SegmentedConsole.Console;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            var Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var Random = new Random();
            Console.GetSegment("R").Write("HELLO WORLD!");
            Console.GetInputSegment().LineEntered += OnEntered;
            // Keep alive
            while (true)
            {
                System.Threading.Thread.Sleep(250);
                Console.GetSegment("L").Write(Characters.ElementAt(Random.Next(Characters.Length)).ToString());
            }
        }

        private static void OnEntered(string RawValue)
        {
            var Value = RawValue;
            var Target = RawValue.Substring(0,1);
            if (Target != "R" && Target != "L")
            {
                Target = "L";
            }
            else
            {
                Value = RawValue.Substring(1);
            }
            Console.GetSegment(Target).Write(Value);
        }
    }
}
