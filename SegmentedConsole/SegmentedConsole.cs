using System;
using System.Threading;
using System.Collections.Immutable;
using SysConsole = System.Console;
using System.Linq;

namespace SegmentedConsole
{
    public class Console
    {
        private static readonly IntPtr STDInHandle;
        internal static readonly IntPtr STDOutHandle;
        private static readonly char[] ConsoleBuffer;
        private static IImmutableDictionary<string, ISegment> Segments;
        private static InputSegment Input;
        private static Timer InputTimer;
        internal const string InternalPrefix = "$__";

        static Console()
        {
            Initialize();
            STDInHandle = Native.GetStdHandle(-10);
            STDOutHandle = Native.GetStdHandle(-11);
            ConsoleBuffer = new char[SysConsole.WindowHeight * SysConsole.WindowWidth];
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
            if (SysConsole.KeyAvailable)
            {
                Input?.OnKeyAvailable();
            }
        }

        public static ISegment GetOutputSegment(string Name)
        {
            if(Name.StartsWith(InternalPrefix))
            {
                return null;
            }
            return Segments[Name];
        }

        public static void ApplyLayout(LayoutBuilder Layout)
        {
            Segments = Layout.Segments.ToImmutableDictionary((pair) => pair.Key, (pair) => (ISegment)pair.Value);
            Input = (InputSegment)Segments.Where((pair) => pair.Value is InputSegment).SingleOrDefault().Value;
        }
    }
}
