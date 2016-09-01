using System;
using System.Collections.Immutable;
using System.Linq;

namespace SegmentedConsole
{
    public sealed class LayoutBuilder
    {
        private const string ReservedPrefix = "$$";
        private const string LineIdentifier = ReservedPrefix  + "_LINE_";
        internal ImmutableDictionary<string, Segment> Segments { get; private set; }

        public LayoutBuilder()
        {
            this.Segments = ImmutableDictionary<string, Segment>.Empty;
        }

        private LayoutBuilder(ImmutableDictionary<string, Segment> OldSegmnents, string Name, Segment NewSegment)
        {
            this.Segments = OldSegmnents.Add(Name, NewSegment);
        }

        private void CheckName(string Name)
        {
            if(Name.StartsWith(ReservedPrefix))
            {
                throw new ArgumentException($"Name '{Name}' is not allowed. Names can not begin with '{ReservedPrefix}'");
            }
            if(Segments.ContainsKey(Name))
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

        private string GenerateNewLineName()
        {
            var LineCount = Segments.Keys.Where(s => s.StartsWith(LineIdentifier)).Count();
            return $"{LineIdentifier}{LineCount}";
        }

        public LayoutBuilder AddOutputSegment(string Name, int X, int Y, int Width, int Height)
        {
            CheckName(Name);
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
            CheckName(Name);
            var NewSegment = new InputSegment(new Coord(Y, X), Width, Height);
            CheckForSegmentIntersection(Name, NewSegment);
            return new LayoutBuilder(this.Segments, Name, NewSegment);
        }

        public LayoutBuilder AddVerticalLine(int X, int Y, int Height, char Line = '|')
        {
            var NewSegment = new OutputSegment(new Coord(Y, X), 1, Height);
            var Name = GenerateNewLineName();
            CheckForSegmentIntersection(Name, NewSegment);
            NewSegment.Write(new string(Line, Height));
            return new LayoutBuilder(this.Segments, Name, NewSegment);
        }

        public LayoutBuilder AddHorizontalLine(int X, int Y, int Width, char Line = '-')
        {
            throw new NotImplementedException();
        }

        public LayoutBuilder AddBorder()
        {
            throw new NotImplementedException();
        }
    }
}
