using System.Collections.Generic;

namespace Iridium.Geo
{
    public interface IIntersectable<in T> where T : IGeometry
    {
        bool Intersects(T other);
        IEnumerable<Point> Intersections(T other);
    }
}