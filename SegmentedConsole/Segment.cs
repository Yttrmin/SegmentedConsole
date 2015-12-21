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
        public Coord Bounds { get; private set; }
        public Rect Area { get; private set; }
        public int Width => Area.Right - Area.Left;
        public int Height => Area.Bottom - Area.Top;

        protected Segment(Rect Area)
        {
            this.Area = Area;
            // Row, Column
            Buffer = new CharInfo[Height+1, Width+1];
            Cursor = new Coord(0,0);
            Bounds = new Coord(Width, Height);
        }

        public void Write(string Text)
        {
            var Chars = Text.ToCharArray();
            foreach(var Char in Chars)
            {
                Buffer[Cursor.Row, Cursor.Column] = new CharInfo(Char);
                Cursor = Cursor.Advance(Bounds);
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
            throw new NotImplementedException();
        }
    }

    internal sealed class InputSegment : Segment
    {
        public InputSegment(Rect Area)
            : base(Area)
        {

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
