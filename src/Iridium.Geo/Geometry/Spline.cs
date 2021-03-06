using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class Spline : ITransformable<Spline>, IMultiGeometry<ILinearGeometry>, ILinearGeometry, IClosedGeometry
    {
        public IReadOnlyList<ILinearGeometry> Curves { get; }
        public bool Closed { get; }

        private Rectangle _boundingBox;

        public Spline(IEnumerable<ILinearGeometry> curves, bool closed)
        {
            Curves = curves.ToArray();
            Closed = closed;
        }

        public Point ClosestPoint(Point p, out double minDistance)
        {
            Point closest = null;
            minDistance = double.MaxValue;

            foreach (var curve in Curves)
            {
                Point cp = curve.ClosestPoint(p);

                var distance = p.DistanceTo(cp);

                if (distance < minDistance)
                {
                    closest = cp;
                    minDistance = distance;
                }
            }

            return closest;
        }

        public Spline Scale(double factor, Point origin = null)
        {
            return new Spline(Curves.Scale(factor, origin), Closed);
        }

        public Spline Translate(double dx, double dy)
        {
            return new Spline(Curves.Translate(dx, dy), Closed);
        }

        public Spline Rotate(double angle, Point origin = null)
        {
            return new Spline(Curves.Rotate(angle, origin), Closed);
        }

        public Spline Transform(AffineMatrix2D matrix)
        {
            return new Spline(Curves.Transform(matrix), Closed);
        }


        public Rectangle BoundingBox()
        {
            if (_boundingBox != null)
                return _boundingBox;

            double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

            foreach (var curve in Curves)
            {
                var box = curve.BoundingBox();

                x1 = Math.Min(x1, box.MinX);
                y1 = Math.Min(y1, box.MinY);
                x2 = Math.Max(x2, box.MaxX);
                y2 = Math.Max(y2, box.MaxY);
            }

            return (_boundingBox = new Rectangle(new Point(x1, y1), new Point(x2, y2)));
        }

        public Point ClosestPoint(Point p)
        {
            return ClosestPoint(p, out var distance);
        }


        public IEnumerator<ILinearGeometry> GetEnumerator()
        {
            return Curves.GetEnumerator();
        }

        public bool Intersects(ILinearGeometry other)
        {
            return Curves.Any(c => c.Intersects(other));
        }

        public IEnumerable<Point> Intersections(ILinearGeometry other)
        {
            return Curves.SelectMany(c => c.Intersections(other));
        }

        public double Length => Curves.Sum(c => c.Length);
        public double StartAngle => Curves.First().StartAngle;
        public double EndAngle => Curves.Last().EndAngle;
        public Point StartPoint => Curves.First().StartPoint;
        public Point EndPoint => Curves.Last().EndPoint;

        public bool Overlaps(IGeometry other)
        {
            throw new NotImplementedException();
        }

        public double Area => throw new NotImplementedException();

        public bool IsPointInside(Point p)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        ILinearGeometry IScalable<ILinearGeometry>.Scale(double factor, Point origin) => Scale(factor, origin);
        ILinearGeometry IRotatable<ILinearGeometry>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ILinearGeometry ITranslatable<ILinearGeometry>.Translate(double dx, double dy) => Translate(dx, dy);
        ILinearGeometry ITransformable<ILinearGeometry>.Transform(AffineMatrix2D matrix) => Transform(matrix);
    }

}
