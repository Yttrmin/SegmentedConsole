using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Immutable;
using SysConsole = System.Console;
using static System.Math;

namespace SegmentedConsole
{
    public class Console
    {
        private static readonly IntPtr STDInHandle;
        internal static readonly IntPtr STDOutHandle;
        private static readonly char[] ConsoleBuffer;
        private static IImmutableDictionary<string, OutputSegment> Outputs;
        private static InputSegment In;
        private static Timer InputTimer;
        //@DELETEME
        private static int Ticks;

        static Console()
        {
            Initialize();
            STDInHandle = Native.GetStdHandle(-10);
            STDOutHandle = Native.GetStdHandle(-11);
            ConsoleBuffer = new char[SysConsole.WindowHeight * SysConsole.WindowWidth];
            var Outputs = new Dictionary<string, OutputSegment>();
            Outputs["L"] = new OutputSegment(new Coord(1,1), 8, 3);
            Outputs["R"] = new OutputSegment(new Coord(1, 15), 8, 3);
            Outputs["DB"] = new DataBoundSegment(new SegmentedConsole.Coord(1, 25), 40, 1, 10,
                "A number: {0}  ;  Number/3: {1}", () => Ticks.ToString(), () => Ticks/3);
            Console.Outputs = Outputs.ToImmutableDictionary();
            In = new InputSegment(new Coord(15,0), 5, 2);
            InputTimer = new Timer(Tick, null, 0, 1000/60);
        }
        
        private static void Initialize()
        {
            // Create console if one does not exist.
            var ConsoleHandle = Native.GetConsoleWindow();
            if (ConsoleHandle == IntPtr.Zero)
            {
                var ConsoleAllocated = Native.AllocConsole();
                ConsoleHandle = Native.GetConsoleWindow();
                if (!ConsoleAllocated || ConsoleHandle == IntPtr.Zero)
                {
                    throw new Exception("No console existed and a new one could not be allocated.");
                }
            }
        }

        private static void Tick(object State)
        {
            Ticks++;
            if (SysConsole.KeyAvailable)
            {
                In.OnKeyAvailable();
            }
        }

        public static void Create(params Segment[] Segments)
        {
            throw new NotImplementedException();
        }

        public static OutputSegment GetSegment(string Name)
        {
            return Outputs[Name];
        }

        public static InputSegment GetInputSegment()
        {
            return In;
        }
    }
}
