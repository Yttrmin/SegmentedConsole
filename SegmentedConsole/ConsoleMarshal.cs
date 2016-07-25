using System;
using System.Runtime.InteropServices;

namespace SegmentedConsole
{
    internal static class Native
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        /* Writes character and color attribute data to a specified rectangular block of character cells in a console screen buffer.
           The data to be written is taken from a correspondingly sized rectangular block at a specified location in the source buffer. */
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteConsoleOutput(
           IntPtr hConsoleOutput,
           /* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
               whose size is specified by the dwBufferSize parameter.*/
           [MarshalAs(UnmanagedType.LPArray), In] CharInfo[,] lpBuffer,
           Coord dwBufferSize,
           Coord dwBufferCoord,
           ref Rect lpWriteRegion);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
    }

    [Flags]
    internal enum ConsoleAttributes : short
    {
        NONE = 0 << 0,
        FOREGROUND_BLUE = 1 << 0,
        FOREGROUND_GREEN = 1 << 1,
        FOREGROUND_RED = 1 << 2,
        FOREGROUND_INTENSITY = 1 << 3,
        BACKGROUND_BLUE = 1 << 4,
        BACKGROUND_GREEN = 1 << 5,
        BACKGROUND_RED = 1 << 6,
        BACKGROUND_INTENSITY = 1 << 7,
        COMMON_LVB_LEADING_BYTE = 1 << 8,
        COMMON_LVB_TRAILING_BYTE = 1 << 9,
        COMMON_LVB_GRID_HORIZONTAL = 1 << 10,
        COMMON_LVB_GRID_LVERTICAL = 1 << 11,
        COMMON_LVB_GRID_RVERTICAL = 1 << 12,
        COMMON_LVB_REVERSE_VIDEO = 1 << 13,
        COMMON_LVB_UNDERSCORE = 1 << 14,

        //Custom
        FOREGROUND_WHITE = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CharInfo
    {
        public readonly char Char;
        public readonly ConsoleAttributes Attributes;
        public static readonly CharInfo Blank = new CharInfo('©');

        public CharInfo(char Char)
        {
            this.Char = Char;
            // NONE results in a black foreground and background, so invisible.
            this.Attributes = ConsoleAttributes.FOREGROUND_WHITE;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Coord
    {
        public readonly short Column, Row;
        public static readonly Coord Zero = new Coord(0, 0);

        public Coord(short Row, short Column)
        {
            this.Column = Column;
            this.Row = Row;
        }

        public Coord(int Row, int Column)
            :this((short)Row, (short)Column)
        {
        }

        public override string ToString()
        {
            return $"({Row},{Column})";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public readonly short Left, Top, Right, Bottom;

        public Rect(short Left, short Top, short Right, short Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }

        public Rect(int Left, int Top, int Right, int Bottom)
        {
            this.Left = (short)Left;
            this.Top = (short)Top;
            this.Right = (short)Right;
            this.Bottom = (short)Bottom;
        }

        public override string ToString()
        {
            return $"UL:({Left},{Top})   LR:({Right},{Bottom})";
        }
    }
}
