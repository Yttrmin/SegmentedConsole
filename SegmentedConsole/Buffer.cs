using System;

namespace SegmentedConsole
{
    class Buffer
    {
        private readonly bool Scrollable;
        private CharInfo[,] Data;
        private Coord CurrentCoord;
        private bool NeedNewLine => CurrentCoord.Column == Data.GetLength(1);
        private bool NeedScroll => CurrentCoord.Row == Data.GetLength(0);
        private int Width => Data.GetLength(1);
        private int Height => Data.GetLength(0);
        private bool EndOfBuffer => !Scrollable && NeedScroll;

        public CharInfo this[Coord Coord] => Data[Coord.Row, Coord.Column];
        public CharInfo[,] InternalBuffer => (CharInfo[,])Data.Clone();

        public Buffer(int Width, int Height, bool Scrollable)
        {
            if(Width <= 0 || Height <= 0)
            {
                throw new Exception("Dimensions must be positive.");
            }
            Data = new CharInfo[Height, Width];
            this.Scrollable = Scrollable;
        }

        public void Append(CharInfo Char)
        {
            // Only occurs on non-scrollable buffers
            if(EndOfBuffer)
            {
                return;
            }
            if (NeedNewLine)
            {
                Newline();
                Append(Char);
            }
            else
            {
                Data[CurrentCoord.Row, CurrentCoord.Column] = Char;
                CurrentCoord = new Coord(CurrentCoord.Row, CurrentCoord.Column + 1);
            }
        }

        public void Clear()
        {
            Array.Clear(Data, 0, Data.Length);
            CurrentCoord = Coord.Zero;
        }

        /// <summary>
        /// Sets cursor to beginning of the buffer.
        /// This prevents flicker when overwriting, which could happen if Clear() was used.
        /// </summary>
        public void ResetCursor()
        {
            CurrentCoord = Coord.Zero;
        }

        private void Newline()
        {
            CurrentCoord = new Coord(CurrentCoord.Row + 1, 0);
            if(Scrollable && NeedScroll)
            {
                Scroll();
            }
        }

        /// <summary>
        /// Move the data up one row, overwriting the top row, and clearing the bottom row.
        /// </summary>
        private void Scroll()
        {
            Array.ConstrainedCopy(Data, 1 * Width, Data, 0, Width * (Height - 1));
            //Array.Clear(Data, Width * (Height - 1), Width);
            ClearRowToChar(Height-1, CharInfo.Blank);
            CurrentCoord = new Coord(CurrentCoord.Row - 1, CurrentCoord.Column);
        }

        private void ClearRowToChar(int Row, CharInfo Char)
        {
            for (var i = 0; i < Width; i++)
            {
                Data[Row, i] = Char;
            }
        }
    }
}
