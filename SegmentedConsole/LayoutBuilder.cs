using System;
using System.Collections.Immutable;
using System.Linq;

namespace SegmentedConsole
{
    public sealed class LayoutBuilder
    {
        internal ImmutableDictionary<string, Segment> Segments { get; private set; }

        public LayoutBuilder()
        {
            this.Segments = ImmutableDictionary<string, Segment>.Empty;
        }

        private LayoutBuilder(ImmutableDictionary<string, Segment> OldSegmnents, string Name, Segment NewSegment)
        {
            this.Segments = OldSegmnents.Add(Name, NewSegment);
        }

        private void CheckForDuplicateName(string Name)
        {
            if (Segments.ContainsKey(Name))
            {
                throw new ArgumentException($"There is already a Segment called '{Name}' in the layout.");
            }
        }

        private void CheckForSegmentIntersection(string Name, Segment ToCheck)
        {
            foreach(var ExistingSegment in Segments)
            {
                if(Segment.Intersect(ExistingSegment.Value, ToCheck))
                {
                    throw new ArgumentException($"Can not add Segment '{Name}', it intersects with existing Segment '{ExistingSegment.Key}'");
                }
            }
        }

        public LayoutBuilder AddOutputSegment(string Name, int X, int Y, int Width, int Height)
        {
            CheckForDuplicateName(Name);
            var NewSegment = new OutputSegment(new Coord(Y, X), Width, Height);
            CheckForSegmentIntersection(Name, NewSegment);
            return new LayoutBuilder(this.Segments, Name, NewSegment);
        }

        public LayoutBuilder AddDataBoundSegment(string Name, int X, int Y, int Width, int Height, string Template, 
            params Func<string>[] Data)
        {
            throw new NotImplementedException();
        }

        //@TODO - Remove name, autogenerate it.
        public LayoutBuilder AddInputSegment(string Name, int X, int Y, int Width, int Height)
        {
            var ExistingInputSegment = Segments.Where((pair) => pair.Value is InputSegment).SingleOrDefault().Key;
            if(ExistingInputSegment != null)
            {
                throw new ArgumentException($"Layouts may only have a maximum of 1 InputSegment. There is already an InputSegment named '{ExistingInputSegment}'");
            }
            CheckForDuplicateName(Name);
            var NewSegment = new InputSegment(new Coord(Y, X), Width, Height);
            CheckForSegmentIntersection(Name, NewSegment);
            return new LayoutBuilder(this.Segments, Name, NewSegment);
        }

        public LayoutBuilder AddVerticalLine(int X, int Y, int Height=int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public LayoutBuilder AddHorizontalLine(int X, int Y, int Width=int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public LayoutBuilder AddBorder()
        {
            throw new NotImplementedException();
        }
    }
}
