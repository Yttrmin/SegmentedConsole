﻿using System;
using System.Text;
using SysConsole = System.Console;
using System.Collections.Immutable;
using System.Threading;
using System.Linq;

namespace SegmentedConsole
{
    public class Segment
    {
        //@TODO - Double buffering, only update dirty portions?
        internal Buffer Buffer { get; private set; }
        /// <summary>
        /// The inclusive area reserved on the console window for this segment.
        /// </summary>
        internal Rect Area { get; private set; }
        internal Coord Size { get; }
        internal int Width => Area.Right - Area.Left;
        internal int Height => Area.Bottom - Area.Top;

        internal Segment(Coord UpperLeft, int Width, int Height, bool Scrollable)
        {
            Area = new Rect(UpperLeft.Column, UpperLeft.Row, UpperLeft.Column + Width, UpperLeft.Row + Height);
            Size = new Coord(Height, Width);
            // Row, Column
            Buffer = new Buffer(Width, Height, Scrollable);
        }

        public void Write(string Text)
        {
            var Chars = Text.ToCharArray();
            foreach(var Char in Chars)
            {
                Buffer.Append(Char);
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

        public void WriteLine(string Text)
        {
            throw new NotImplementedException();
        }

        public void Resize()
        {
            throw new NotImplementedException();
        }

        private void WriteToConsole()
        {
            var Area = this.Area;
            Native.WriteConsoleOutput(Console.STDOutHandle, this.Buffer.InternalBuffer, this.Size, Coord.Zero, ref Area);
        }
    }

    public sealed class InputSegment : Segment
    {
        private StringBuilder Builder;
        public event Action<string> LineEntered;

        public InputSegment(Coord UpperLeft, int Width, int Height)
            : base(UpperLeft, Width, Height, true)
        {
            SysConsole.SetCursorPosition(Area.Left, Area.Top);
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
    
    public class OutputSegment : Segment
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

    public sealed class DataBoundSegment : OutputSegment
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
