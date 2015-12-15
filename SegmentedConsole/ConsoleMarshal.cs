using System;
using System.Runtime.InteropServices;

namespace SegmentedConsole
{
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
        public readonly short X, Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        short Left, Top, Right, Bottom;

        public Rect(short Left, short Top, short Right, short Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }
    }
}
