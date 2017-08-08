using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiPoint : IMultiGeometry<Point>, IIntersectable<Point>, IIntersectable<MultiPoint>
    {
        public IReadOnlyList<Point> Points { get; }

        public MultiPoint(IEnumerable<Point> points)
        {
            Points = points.ToArray();
        }

        IGeometry IGeometry.Rotate(double angle, Point origin)
        {
            return Rotate(angle, origin);
        }

        IGeometry IGeometry.Scale(double factor, Point origin)
        {
            return Scale(factor, origin);
        }

        IGeometry IGeometry.Transform(AffineMatrix2D matrix)
        {
            return Transform(matrix);
        }

        public Point ClosestPoint(Point p)
        {
            return Points.ClosestPoint(p);
        }

        public MultiPoint Scale(double factor, Point origin = null)
        {
            return new MultiPoint(Points.Scale(factor, origin));
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }

        public MultiPoint Rotate(double angle, Point origin = null)
        {
            return new MultiPoint(Points.Rotate(angle, origin));
        }

        public Rectangle BoundingBox()
        {
            return Points.BoundingBox();
        }

        public MultiPoint Translate(double dx, double dy)
        {
            return new MultiPoint(Points.Translate(dx, dy));
        }

        public MultiPoint Transform(AffineMatrix2D matrix)
        {
            return new MultiPoint(Points.Transform(matrix));
        }

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return Points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Intersects(Point other)
        {
            return Points.Any(pt => pt.X == other.X && pt.Y == other.Y);
        }

        public IEnumerable<Point> Intersections(Point other)
        {
            return Points.Where(pt => pt.X == other.X && pt.Y == other.Y);
        }

        public bool Intersects(MultiPoint other)
        {
            return Points.Any(pt1 => other.Any(pt2 => pt2.X == pt1.X && pt2.Y == pt1.Y));
        }

        public IEnumerable<Point> Intersections(MultiPoint other)
        {
            return Points.SelectMany(pt1 => other.Where(pt2 => pt2.X == pt1.X && pt2.Y == pt1.Y));
        }
    }
}