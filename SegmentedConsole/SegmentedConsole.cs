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
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        private static readonly IntPtr STDInHandle;
        private static readonly char[] Divider;
        private static readonly char[] ConsoleBuffer;
        private static OutputSegment Out;

        static Console()
        {
            Initialize();
            //STDInHandle = GetStdHandle(-11);
            //Divider = new string('=', SysConsole.BufferWidth).ToCharArray();
            ConsoleBuffer = new char[SysConsole.WindowWidth * SysConsole.WindowHeight];
            SysConsole.ReadLine();
            Out = new OutputSegment(SysConsole.WindowWidth, SysConsole.WindowHeight);
        }
        
        private static void Initialize()
        {
            // Create console if one does not exist.
            var ConsoleHandle = GetConsoleWindow();
            if (ConsoleHandle == IntPtr.Zero)
            {
                var ConsoleAllocated = AllocConsole();
                ConsoleHandle = GetConsoleWindow();
                if (!ConsoleAllocated || ConsoleHandle == IntPtr.Zero)
                {
                    throw new Exception("No console existed and a new one could not be allocated.");
                }
            }
            var TokenSource = new CancellationTokenSource();
            Task.Factory.StartNew((t) => UpdateConsole((CancellationToken)t), TokenSource.Token, TaskCreationOptions.LongRunning);
        }

        private static async Task UpdateConsole(CancellationToken Token)
        {
            while(!Token.IsCancellationRequested)
            {
                await Task.Delay(1000/60);
                Console.Write("A");
            }
        }

        public static void Write(string Text)
        {
            Out.Write(Text);
            Out.MergeBuffer(ConsoleBuffer);
            SysConsole.SetCursorPosition(0, 0);
            SysConsole.Write(ConsoleBuffer);
        }

        public static void Poke() { }
    }
}
