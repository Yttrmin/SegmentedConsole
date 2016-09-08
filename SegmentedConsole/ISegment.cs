using System;

namespace SegmentedConsole
{
    public interface ISegment
    {
        void Write(string Text, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black);
        void WriteLine(string Text, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black);
        void Clear();
    }
}
