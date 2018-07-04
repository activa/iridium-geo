using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo.Drawing
{
    public class Gradient
    {
        public class Stop
        {
            public readonly Color Color;
            public readonly double Position;

            public Stop(Color color, double position)
            {
                Color = color;
                Position = position;
            }
        }

        public readonly Stop[] Stops;
        public readonly LineSegment Line;

        public Gradient(IEnumerable<Stop> stops, LineSegment line)
        {
            Stops = stops.ToArray();
            Line = line;
        }

        public Gradient Colorize(Color c)
        {
            return new Gradient(Stops.Select(stop => new Stop(stop.Color.Colorize(c), stop.Position)), Line);
        }
    }
}