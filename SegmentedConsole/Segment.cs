using System;
using System.Text;
using SysConsole = System.Console;
using System.Collections.Immutable;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace SegmentedConsole
{
    internal class Segment : ISegment
    {
        private static readonly ImmutableDictionary<ConsoleColor, ConsoleAttributes> ConsoleColorMap;
        //@TODO - Double buffering, only update dirty portions?
        internal Buffer Buffer { get; private set; }
        /// <summary>
        /// The inclusive area reserved on the console window for this segment.
        /// </summary>
        internal Rect Area { get; private set; }
        internal Coord Size { get; }
        internal int Width => Area.Right - Area.Left;
        internal int Height => Area.Bottom - Area.Top;

        static Segment()
        {
            ConsoleColorMap = new Dictionary<ConsoleColor, ConsoleAttributes>
            {
                { ConsoleColor.Black,       ConsoleAttributes.NONE },
                { ConsoleColor.DarkBlue,    ConsoleAttributes.FOREGROUND_BLUE },
                { ConsoleColor.DarkGreen,   ConsoleAttributes.FOREGROUND_GREEN },
                { ConsoleColor.DarkCyan,    ConsoleAttributes.FOREGROUND_BLUE  | ConsoleAttributes.FOREGROUND_GREEN },
                { ConsoleColor.DarkRed,     ConsoleAttributes.FOREGROUND_RED },
                { ConsoleColor.DarkMagenta, ConsoleAttributes.FOREGROUND_BLUE  | ConsoleAttributes.FOREGROUND_RED },
                { ConsoleColor.DarkYellow,  ConsoleAttributes.FOREGROUND_GREEN | ConsoleAttributes.FOREGROUND_RED },
                { ConsoleColor.Gray,        ConsoleAttributes.FOREGROUND_RGB },
                { ConsoleColor.DarkGray,    ConsoleAttributes.FOREGROUND_INTENSITY },
                { ConsoleColor.Blue,        ConsoleAttributes.FOREGROUND_BLUE  | ConsoleAttributes.FOREGROUND_INTENSITY},
                { ConsoleColor.Green,       ConsoleAttributes.FOREGROUND_GREEN | ConsoleAttributes.FOREGROUND_INTENSITY},
                { ConsoleColor.Cyan,        ConsoleAttributes.FOREGROUND_BLUE  | ConsoleAttributes.FOREGROUND_GREEN | ConsoleAttributes.FOREGROUND_INTENSITY },
                { ConsoleColor.Red,         ConsoleAttributes.FOREGROUND_RED   | ConsoleAttributes.FOREGROUND_INTENSITY },
                { ConsoleColor.Magenta,     ConsoleAttributes.FOREGROUND_BLUE  | ConsoleAttributes.FOREGROUND_RED   | ConsoleAttributes.FOREGROUND_INTENSITY },
                { ConsoleColor.Yellow,      ConsoleAttributes.FOREGROUND_GREEN | ConsoleAttributes.FOREGROUND_RED   | ConsoleAttributes.FOREGROUND_INTENSITY },
                { ConsoleColor.White,       ConsoleAttributes.FOREGROUND_RGB   | ConsoleAttributes.FOREGROUND_INTENSITY}
            }.ToImmutableDictionary();
        }

        private static ConsoleAttributes ConvertColor(ConsoleColor Color, bool ForegroundColor)
        {
            if(ForegroundColor)
            {
                return ConsoleColorMap[Color];
            }
            else
            {
                return (ConsoleAttributes)((short)ConsoleColorMap[Color] << 4);
            }
        }

        internal Segment(Coord UpperLeft, int Width, int Height, bool Scrollable)
        {
            Area = new Rect(UpperLeft.Column, UpperLeft.Row, UpperLeft.Column + Width, UpperLeft.Row + Height);
            Size = new Coord(Height, Width);
            // Row, Column
            Buffer = new Buffer(Width, Height, Scrollable);
        }

        public void Write(string Text, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black)
        {
            var Chars = Text.ToCharArray();
            foreach(var Char in Chars)
            {
                var ColorAttributes = ConvertColor(ForegroundColor, true) | ConvertColor(BackgroundColor, false);
                Buffer.Append(new CharInfo(Char, ColorAttributes));
            }
            if(Chars.Length > 0)
            {
                WriteToConsole();
            }
        }

        public void Clear()
        {
            Buffer.Clear();
            WriteToConsole();
        }

        public void WriteLine(string Text, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black)
        {
            throw new NotImplementedException();
        }

        public void Resize()
        {
            throw new NotImplementedException();
        }

        internal bool Intersects(Segment Other)
        {
            return this.Area.Left < Other.Area.Right && this.Area.Right > Other.Area.Left
                && this.Area.Top < Other.Area.Bottom && this.Area.Bottom > Other.Area.Top;
        }

        //@TODO - Need a way to keep inactive layouts from writing to the console.
        private void WriteToConsole()
        {
            var Area = this.Area;
            Native.WriteConsoleOutput(Console.STDOutHandle, this.Buffer.InternalBuffer, this.Size, Coord.Zero, ref Area);
        }
    }

    internal sealed class InputSegment : Segment
    {
        private StringBuilder Builder;
        public event Action<string> LineEntered;

        public InputSegment(Coord UpperLeft, int Width, int Height)
            : base(UpperLeft, Width, Height, true)
        {
            //@TODO - Constructed in layout, can't mess with cursor position until it's set.
            //SysConsole.SetCursorPosition(Area.Left, Area.Top);
            this.Builder = new StringBuilder();
        }

        public void OnKeyAvailable()
        {
            while(SysConsole.KeyAvailable)
            {
                var Info = SysConsole.ReadKey(true);
                if(IgnoreValue(Info))
                {
                    continue;
                }
                if(Info.Key == ConsoleKey.Enter)
                {
                    LineEntered?.Invoke(Builder.ToString());
                    Builder.Clear();
                }
                else if(Info.Key == ConsoleKey.Backspace)
                {
                    if (Builder.Length > 0)
                    {
                        Builder.Remove(Builder.Length - 1, 1);
                    }
                }
                else
                {
                    Builder.Append(Info.KeyChar);
                }
            }
            // Not particularly efficient but probably fine.
            Clear();
            Write(Builder.ToString());
        }

        private bool IgnoreValue(ConsoleKeyInfo Info)
        {
            if ((Info.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
            {
                return true;
            }
            if ((Info.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
            {
                return true;
            }
            // Ignore if KeyChar value is \u0000.
            if (Info.KeyChar == '\u0000') return true;
            // Ignore tab key.
            if (Info.Key == ConsoleKey.Tab) return true;

            return false;
        }
    }

    internal class OutputSegment : Segment
    {
        public OutputSegment(Coord UpperLeft, int Width, int Height)
            : this(UpperLeft, Width, Height, true)
        {

        }

        protected OutputSegment(Coord UpperLeft, int Width, int Height, bool Scrollable)
            : base(UpperLeft, Width, Height, Scrollable)
        {

        }
    }

    internal sealed class DataBoundSegment : OutputSegment
    {
        private readonly Timer PollingTimer;
        private readonly string Template;
        private ImmutableArray<Func<object>> BoundData;

        public DataBoundSegment(Coord UpperLeft, int Width, int Height, 
            int PollingIntervalMilliseconds, string Template, params Func<object>[] BoundData)
            : base(UpperLeft, Width, Height, false)
        {
            this.Template = Template;
            this.BoundData = BoundData.ToImmutableArray();
            this.PollingTimer = new Timer(Poll, null, 0, PollingIntervalMilliseconds);
        }

        private void Poll(object State)
        {
            var FinalString = String.Format(Template, BoundData.Select((func) => func()).ToArray());
            lock (Buffer)
            {
                Buffer.ResetCursor();
                this.Write(FinalString);
            }
        }
    }
}
