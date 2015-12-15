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
        private char[] Buffer;
        private int Cursor;
        private int X, Y;

        protected Segment(int Width, int Height)
        {
            Buffer = new char[Width * Height];
            Cursor = 0;
            X = Y = 0;
        }

        public void Write(string Text)
        {
            var Chars = Text.ToCharArray();
            Array.ConstrainedCopy(Chars, 0, Buffer, Cursor, Chars.Length);
            Cursor += Chars.Length;
        }

        public void WriteLine(string Text)
        {
            throw new NotImplementedException();
        }

        public void MergeBuffer(char[] DestBuffer)
        {
            // This is not at all right for segments narrower than the console.
            Array.ConstrainedCopy(Buffer, X * Y, DestBuffer, X * Y, Buffer.Length);
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
        public InputSegment(int Width, int Height)
            : base(Width, Height)
        {

        }
    }
    
    internal sealed class OutputSegment : Segment
    {
        public OutputSegment(int Width, int Height)
            : base(Width, Height)
        {

        }
    }
}
