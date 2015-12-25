using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysConsole = System.Console;

namespace SegmentedConsole
{
    internal class Segment
    {
        public CharInfo[,] Buffer { get; private set; }
        private Coord Cursor;
        /// <summary>
        /// Size of the Buffer
        /// </summary>
        public Coord Bounds { get; private set; }
        /// <summary>
        /// The inclusive area reserved on the console window for this segment.
        /// </summary>
        public Rect Area { get; private set; }
        public Coord Size { get; }
        public int Width => Area.Right - Area.Left;
        public int Height => Area.Bottom - Area.Top;
        private bool PendingNewLine;

        protected Segment(Rect Area)
        {
            this.Area = Area;
            var BufferWidth = Width+1;
            var BufferHeight = Height+1;
            Size = new Coord(BufferWidth, BufferHeight);
            // Row, Column
            Buffer = new CharInfo[Size.Row, Size.Column];
            Cursor = new Coord(0,0);
            Bounds = new Coord(Buffer.GetUpperBound(1), Buffer.GetUpperBound(0));
        }

        public void Write(string Text)
        {
            var Chars = Text.ToCharArray();
            foreach(var Char in Chars)
            {
                if(PendingNewLine)
                {
                    NewLine();
                    Cursor = new Coord((short)0, Cursor.Row);
                    PendingNewLine = false;
                }
                Buffer[Cursor.Row, Cursor.Column] = new CharInfo(Char);
                if (!Cursor.CanAdvance(Bounds))
                {
                    PendingNewLine = true;
                }
                else
                {
                    Cursor = Cursor.Advance(Bounds);
                }
            }
        }

        public void WriteLine(string Text)
        {
            throw new NotImplementedException();
        }

        public void Resize()
        {
            throw new NotImplementedException();
        }

        private void NewLine()
        {
            Array.ConstrainedCopy(Buffer, Size.Column, Buffer, 0, Size.Column * (Size.Row - 1));
            // Blank out the last row.
            var row = Buffer.GetUpperBound(0);
            for (var col = 0; col < Size.Column; col++)
            {
                Buffer[row, col] = CharInfo.Blank;
            }
        }
    }

    internal sealed class InputSegment : Segment
    {
        private StringBuilder Builder;
        public event Action<string> LineEntered;

        public InputSegment(Rect Area)
            : base(Area)
        {
            SysConsole.SetCursorPosition(Area.Left, Area.Top);
            this.Builder = new StringBuilder();
        }

        public void OnKeyAvailable()
        {
            while(SysConsole.KeyAvailable)
            {
                var Info = SysConsole.ReadKey(false); //@TODO - true
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
                    Builder.Remove(Builder.Length - 1, 1);
                }
                else
                {
                    Builder.Append(Info.KeyChar);
                }
            }
        }

        private bool IgnoreValue(ConsoleKeyInfo Info)
        {
            if ((Info.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                return true;
            if ((Info.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                return true;
            // Ignore if KeyChar value is \u0000.
            if (Info.KeyChar == '\u0000') return true;
            // Ignore tab key.
            if (Info.Key == ConsoleKey.Tab) return true;

            return false;
        }
    }
    
    internal sealed class OutputSegment : Segment
    {
        public OutputSegment(Rect Area)
            : base(Area)
        {

        }
    }
}
