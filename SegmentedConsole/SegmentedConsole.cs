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
        private static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        /* Writes character and color attribute data to a specified rectangular block of character cells in a console screen buffer.
           The data to be written is taken from a correspondingly sized rectangular block at a specified location in the source buffe */
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteConsoleOutput(
           IntPtr hConsoleOutput,
           /* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
               whose size is specified by the dwBufferSize parameter.*/
           [MarshalAs(UnmanagedType.LPArray), In] CharInfo[,] lpBuffer,
           Coord dwBufferSize,
           Coord dwBufferCoord,
           ref Rect lpWriteRegion);

        private static readonly IntPtr STDInHandle;
        private static readonly IntPtr STDOutHandle;
        private static readonly char[] Divider;
        private static readonly char[] ConsoleBuffer;
        private static OutputSegment Out;

        static Console()
        {
            Initialize();
            STDInHandle = GetStdHandle(-10);
            STDOutHandle = GetStdHandle(-11);
            //Divider = new string('=', SysConsole.BufferWidth).ToCharArray();
            ConsoleBuffer = new char[SysConsole.WindowWidth * SysConsole.WindowHeight];
            SysConsole.ReadLine();
            Out = new OutputSegment(SysConsole.WindowWidth, SysConsole.WindowHeight);
            var q = new CharInfo[3, 3];
            q[0, 0] = q[0, 1] = q[0, 2] = new CharInfo('A');
            q[1, 0] = q[1, 1] = q[1, 2] = new CharInfo('B');
            q[2, 0] = q[2, 1] = q[2, 2] = new CharInfo('C');
            var Rect = new Rect(0, 0, 2, 2);
            var Result = WriteConsoleOutput(STDOutHandle, q, new Coord(3, 3), new Coord(0, 0), ref Rect);
            if(!Result)
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
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
                //Console.Write("A");
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
