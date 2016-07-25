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
            var Layout = new SegmentedConsole.LayoutBuilder()
                .AddOutputSegment("L", 1, 1, 8, 3)
                .AddOutputSegment("R", 15, 1, 8, 3)
                .AddInputSegment("IN", 0, 15, 5, 2);
            Console.ApplyLayout(Layout);
            Console.GetOutputSegment("R").Write("HELLO WORLD!");
            // Keep alive
            while (true)
            {
                System.Threading.Thread.Sleep(250);
                Console.GetOutputSegment("L").Write(Characters.ElementAt(Random.Next(Characters.Length)).ToString());
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
            Console.GetOutputSegment(Target).Write(Value);
        }
    }
}
