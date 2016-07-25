using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentedConsole
{
    public interface ISegment
    {
        void Write(string Text);
        void WriteLine(string Text);
        void Clear();
    }
}
