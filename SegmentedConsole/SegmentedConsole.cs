using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SysConsole = System.Console;

namespace SegmentedConsole
{
    public class Console
    {
        private static readonly IntPtr STDInHandle;
        internal static readonly IntPtr STDOutHandle;
        private static readonly char[] ConsoleBuffer;
        private static OutputSegment Out;
        private static InputSegment In;
        private static Timer InputTimer;

        static Console()
        {
            Initialize();
            STDInHandle = Native.GetStdHandle(-10);
            STDOutHandle = Native.GetStdHandle(-11);
            ConsoleBuffer = new char[SysConsole.WindowHeight * SysConsole.WindowWidth];
            var SegmentArea = new Rect(0, 0, 8, 3);
            Out = new OutputSegment(SegmentArea);
            SegmentArea = new Rect(0, 15, 10, 15);
            In = new InputSegment(SegmentArea);
            In.LineEntered += OnEntered;
            InputTimer = new Timer(Tick, null, 0, 1000/60);
        }

        private static void OnEntered(string Value)
        {
            Out.Write(Value);
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
            if (SysConsole.KeyAvailable)
            {
                In.OnKeyAvailable();
            }
        }

        public static void Create(params Segment[] Segments)
        {

        }

        public static void Write(string Text)
        {
            Out.Write(Text);
        }

        public static void Poke() { }
    }
}
