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

        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);
        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);

        public Point ClosestPoint(Point p) => Points.ClosestPoint(p);
        public Rectangle BoundingBox() => Points.BoundingBox();

        public MultiPoint Scale(double factor, Point origin = null) => new MultiPoint(Points.Scale(factor, origin));
        public MultiPoint Rotate(double angle, Point origin = null) => new MultiPoint(Points.Rotate(angle, origin));
        public MultiPoint Translate(double dx, double dy) => new MultiPoint(Points.Translate(dx, dy));
        public MultiPoint Transform(AffineMatrix2D matrix) => new MultiPoint(Points.Transform(matrix));

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Point> GetEnumerator() => Points.GetEnumerator();

        public bool Intersects(Point other) => Points.Any(pt => pt.X == other.X && pt.Y == other.Y);
        public IEnumerable<Point> Intersections(Point other) => Points.Where(pt => pt.X == other.X && pt.Y == other.Y);
        public bool Intersects(MultiPoint other) => Points.Any(pt1 => other.Any(pt2 => pt2.X == pt1.X && pt2.Y == pt1.Y));
        public IEnumerable<Point> Intersections(MultiPoint other) => Points.SelectMany(pt1 => other.Where(pt2 => pt2.X == pt1.X && pt2.Y == pt1.Y));
    }
}