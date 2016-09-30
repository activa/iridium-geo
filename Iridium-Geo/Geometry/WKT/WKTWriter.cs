using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Iridium.Geo
{
    public class WKTWriter
    {
        public static string ToWKT(IGeometry geometry)
        {
            var point = geometry as Point;

            if (point != null)
                return ToWKT(point);

            var polygon = geometry as Polygon;
            
            if (polygon != null)
                return ToWKT(polygon);

            var mPolygon = geometry as MultiPolygon;

            if (mPolygon != null)
                return ToWKT(mPolygon);

            var mPoint = geometry as MultiPoint;

            if (mPoint != null)
                return ToWKT(mPoint);

            throw new Exception($"Can't generate WKT for {geometry?.GetType()?.Name}");
        }

        private static string ToWKT(params double[] numbers)
        {
            return string.Join(" ", numbers.Select(n => n.ToString(NumberFormatInfo.InvariantInfo)));
        }

        private static string ToWKT(IEnumerable<Point> points)
        {
            return string.Join(",", points.Select(p => ToWKT(p.X,p.Y)));
        }

        public static string ToWKT(Point p)
        {
            return $"POINT ({ToWKT(p.X,p.Y)})";
        }

        public static string ToWKT(Polygon polygon)
        {
            return $"LINESTRING ({string.Join(",", polygon.Points.Select(p => ToWKT(p.X,p.Y)))})";
        }

        public static string ToWKT(MultiPolygon multiPolygon)
        {
            return $"MULTILINESTRING (({string.Join("),(",multiPolygon.Polygons.Select(p => ToWKT(p.Points)))}))";
        }

        public static string ToWKT(MultiPoint multiPoint)
        {
            return $"MULTIPOINT ({string.Join(",", multiPoint.Points.Select(p => ToWKT(p.X,p.Y)))})";
        }
    }
}