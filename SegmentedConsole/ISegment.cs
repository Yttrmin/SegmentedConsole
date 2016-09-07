using System;

namespace SegmentedConsole
{
    public interface ISegment
    {
        void Write(string Text, ConsoleColor ForegroundColor=ConsoleColor.White);
        void WriteLine(string Text);
        void Clear();
    }
}
