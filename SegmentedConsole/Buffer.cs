using System;

namespace SegmentedConsole
{
    class Buffer
    {
        private CharInfo[,] Data;
        private Coord CurrentCoord;
        private bool NeedNewLine => CurrentCoord.Column == Data.GetLength(1);
        private bool NeedScroll => CurrentCoord.Row == Data.GetLength(0);
        private int Width => Data.GetLength(1);
        private int Height => Data.GetLength(0);

        public CharInfo this[Coord Coord] => Data[Coord.Row, Coord.Column];
        public CharInfo[,] InternalBuffer => (CharInfo[,])Data.Clone();

        public Buffer(int Width, int Height)
        {
            if(Width <= 0 || Height <= 0)
            {
                throw new Exception("Dimensions must be positive.");
            }
            Data = new CharInfo[Height, Width];
        }

        public void Append(char Character)
        {
            if(NeedNewLine)
            {
                Newline();
                Append(Character);
            }
            else
            {
                Data[CurrentCoord.Row, CurrentCoord.Column] = new CharInfo(Character);
                CurrentCoord = new Coord(CurrentCoord.Row, CurrentCoord.Column + 1);
            }
        }

        public void Clear()
        {
            Array.Clear(Data, 0, Data.Length);
            CurrentCoord = new Coord(0, 0);
        }

        private void Newline()
        {
            CurrentCoord = new Coord(CurrentCoord.Row + 1, 0);
            if(NeedScroll)
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
