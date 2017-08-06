using System;
using System.Collections.Generic;
using System.Linq;
using Iridium.Geo.Geography;

namespace Iridium.Geo
{
    public static class GeometryUtil
    {
        public const double PI2 = Math.PI*2.0;

        public static double ArcLength(double radius, double angle)
        {
            return angle*radius;
        }

        public static double ArcAngle(double radius, double length)
        {
            return length/radius;
        }

        public static double NormalizeAngle(double angle)
        {
            angle = ((angle % PI2) + PI2) % PI2;

            return angle > Math.PI ? (angle - PI2) : angle;
        }

        public static IEnumerable<T> Translate<T>(this IEnumerable<ITranslatable<T>> geometries, double dx, double dy)
        {
            return geometries.Select(p => p.Translate(dx, dy));
        }

        public static IEnumerable<T> Rotate<T>(this IEnumerable<IRotatable<T>> geometries, double angle, Point origin = null)
        {
            return geometries.Select(p => p.Rotate(angle, origin));
        }

        public static IEnumerable<T> Scale<T>(this IEnumerable<IScalable<T>> geometries, double factor, Point origin = null)
        {
            return geometries.Select(p => p.Scale(factor));
        }

        public static IEnumerable<T> Transform<T>(this IEnumerable<ITransformable<T>> geometries, AffineMatrix2D matrix)
        {
            return geometries.Select(p => p.Transform(matrix));
        }

        public static Point ClosestPoint(this IEnumerable<IGeometry> geometries, Point p)
        {
            double distance;

            return ClosestPoint(geometries, p, out distance);
        }

        public static Point ClosestPoint(this IEnumerable<IGeometry> geometries, Point p, out double minDistance)
        {
            Point closest = null;
            minDistance = double.MaxValue;

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

        public static double Length(this IEnumerable<ILinearGeometry> segments)
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

        public static Rectangle BoundingBox(this IEnumerable<Point> points)
        {
            double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

            foreach (var pt in points)
            {
                x1 = Math.Min(x1, pt.X);
                y1 = Math.Min(y1, pt.Y);
                x2 = Math.Max(x2, pt.X);
                y2 = Math.Max(y2, pt.Y);
            }

            if (x1 == double.MaxValue)
                return null;

            return new Rectangle(new Point(x1, y1), new Point(x2, y2));
        }

        public static Rectangle BoundingBox(this IEnumerable<IGeometry> geometries)
        {
            double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

            foreach (var geometry in geometries)
            {
                var box = geometry.BoundingBox();

                x1 = Math.Min(x1, box.MinX);
                y1 = Math.Min(y1, box.MinY);
                x2 = Math.Max(x2, box.MaxX);
                y2 = Math.Max(y2, box.MaxY);
            }

            if (x1 == double.MaxValue)
                return null;

            return new Rectangle(new Point(x1, y1), new Point(x2, y2));
        }

        public static bool Intersects<T1,T2>(this T1 thisGeometry, T2 geometry) where T1:IGeometry where T2:IGeometry
        {
            if (!thisGeometry.BoundingBox().Intersects(geometry.BoundingBox()))
                return false;

            var intersectable1 = thisGeometry as IIntersectable<T2>;

            if (intersectable1 != null)
                return intersectable1.Intersects(geometry);

            var intersectable2 = geometry as IIntersectable<T1>;

            if (intersectable2 != null)
                return intersectable2.Intersects(thisGeometry);

            throw new NotImplementedException();
        }
    }
}
