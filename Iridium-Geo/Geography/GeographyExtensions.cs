using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo.Geography
{
    public static class GeographyExtensions
    {
        public static IEnumerable<LatLon> Project(this IEnumerable<Point> points, GeoProjection projection)
        {
            return points.Select(p => new LatLon(p, projection));
        }

        public static IEnumerable<Point> Project(this IEnumerable<LatLon> points, GeoProjection projection)
        {
            return points.Select(p => p.ToPoint(projection));
        }

        public static IEnumerable<Point> AsPoints(this IEnumerable<LatLon> points)
        {
            return points.Select(p => (Point)p);
        }

        public static IEnumerable<LatLon> AsLatLons(this IEnumerable<Point> latlons)
        {
            return latlons.Select(p => (LatLon)p);
        }
    }
}