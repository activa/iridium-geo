using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Iridium.Geo
{
    public class WKTWriter
    {
        public static string ToWKT(IGeometry geometry)
        {
            var point = geometry as Point;

            if (point != null)
                return ToWKT(point, true);

            var polygon = geometry as Poly;
            
            if (polygon != null)
                return ToWKT(polygon, true);

            var mPolygon = geometry as MultiPolyline;

            if (mPolygon != null)
                return ToWKT(mPolygon, true);

            var mPoint = geometry as MultiPoint;

            if (mPoint != null)
                return ToWKT(mPoint, true);

            throw new Exception($"Can't generate WKT for {geometry?.GetType()?.Name}");
        }

        private static string ToWKT(double x, double y)
        {
            return x.ToString(NumberFormatInfo.InvariantInfo) + " " + y.ToString(NumberFormatInfo.InvariantInfo);
        }

        private static string ToWKT(IEnumerable<Point> points)
        {
            return string.Join(",", points.Select(p => ToWKT(p, false)));
        }

        private static string ToWKT(IEnumerable<Poly> polygons)
        {
            return string.Join(",", polygons.Select(polygon => "("+ToWKT(polygon.Points)+")"));
        }

        private static string ToWKT(Point p, bool includeWrapper)
        {
            return includeWrapper ? $"POINT ({ToWKT(p.X, p.Y)})" : ToWKT(p.X, p.Y);
        }

        private static string ToWKT(Poly polygon, bool includeWrapper)
        {
            return includeWrapper ? $"LINESTRING ({ToWKT(polygon.Points)})" : ToWKT(polygon.Points);
        }

        private static string ToWKT(MultiPolyline multiPolygon, bool includeWrapper)
        {
            return includeWrapper ? $"MULTILINESTRING ({ToWKT(multiPolygon.Polylines)})" : ToWKT(multiPolygon.Polylines);
        }

        private static string ToWKT(MultiPoint multiPoint, bool includeWrapper)
        {
            return includeWrapper ? $"MULTIPOINT ({ToWKT(multiPoint.Points)})" : ToWKT(multiPoint.Points);
        }
    }
}