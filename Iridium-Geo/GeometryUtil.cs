using System;
using System.Collections.Generic;
using System.Linq;
using Iridium.Geo.Geography;

namespace Iridium.Geo
{
    public static class GeometryUtil
    {
        public const double PI2 = Math.PI * 2.0;

        public static double ArcLength(double radius, double angle)
        {
            return angle * radius;
        }

        public static double ArcAngle(double radius, double length)
        {
            return length / radius;
        }

        public static double NormalizeAngle(double angle)
        {
            return angle > 0 ? angle%PI2 : angle%PI2 + PI2;
        }

        public static IEnumerable<T> Translate<T>(this IEnumerable<ITranslatable<T>> geometries,double dx, double dy)
        {
            return geometries.Select(p => p.Translate(dx,dy));
        }

        public static IEnumerable<T> Rotate<T>(this IEnumerable<IRotatable<T>> geometries,double angle, Point origin = null)
        {
            return geometries.Select(p => p.Rotate(angle,origin));
        }

		public static IEnumerable<T> Scale<T>(this IEnumerable<IScalable<T>> geometries, double factor, Point origin = null)
        {
            return geometries.Select(p => p.Scale(factor));
        }

        public static IEnumerable<T> Transform<T>(this IEnumerable<ITransformable<T>> geometries, AffineMatrix2D matrix)
        {
            return geometries.Select(p => p.Transform(matrix));
        }

        public static IEnumerable<LatLon> Project(this IEnumerable<Point> points, GeoProjection projection)
        {
            return points.Select(p => new LatLon(p, projection));
        }

        public static Point ClosestPoint(this IEnumerable<IGeometry> geometries, Point p)
        {
            Point closest = null;
            double minDistance = double.MaxValue;

            foreach (var geometry in geometries)
            {
                var pt = geometry.ClosestPoint(p);
                var distance = pt.DistanceTo(p);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = pt;
                }
            }

            return closest;
        }

        public static double Angle(Point p1, Point p2)
        {
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
        }

        public static double Length(this IEnumerable<LineSegment> segments)
        {
            return segments.Sum(s => s.Length);
        }

        public static double Length(this IEnumerable<Point> points, bool closed = false)
        {
            Point pFirst = null;
            Point pPrev = null;
            var len = 0.0;

            foreach (var p in points)
            {
                if (pPrev != null)
                    len += pPrev.DistanceTo(p);
                else
                    pFirst = p;

                pPrev = p;
            }

            if (closed && pPrev != null)
                len += pPrev.DistanceTo(pFirst);

            return len;
        }

        public static bool IsWithinTolerance(double a, double b)
        {
            return (Math.Abs(a - b) < 0.01);
        }

		public static Rectangle BoundingBox(this IEnumerable<IGeometry> geometries)
        {
            double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

			foreach (var path in geometries)
            {
                var box = path.BoundingBox();

                x1 = Math.Min(x1, box.MinX);
                y1 = Math.Min(y1, box.MinY);
                x2 = Math.Max(x2, box.MaxX);
                y2 = Math.Max(y2, box.MaxY);
            }

            return new Rectangle(new Point(x1, y1), new Point(x2, y2));

        }
    }
}
