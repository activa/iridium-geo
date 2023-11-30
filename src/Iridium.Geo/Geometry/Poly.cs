using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Iridium.Geo
{
    public abstract class Poly : IGeometry, IIntersectable<Poly>, IIntersectable<LineSegment>, ITransformable<Poly>
    {
        public IReadOnlyList<Point> Points { get; }
        public bool Closed { get; }

        private Rectangle _boundingBox;

        protected Poly(IEnumerable<Point> points, bool closed)
        {
            Points = points.ToArray();
            Closed = closed;
        }

        protected abstract Poly CreatePoly(IEnumerable<Point> points);

        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);
        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);

        public Poly Rotate(double angle, Point origin = null) => CreatePoly(Points.Rotate(angle, origin));
        public Poly Scale(double factor, Point origin = null) => CreatePoly(Points.Scale(factor, origin));
        public Poly Transform(AffineMatrix2D matrix) => CreatePoly(Points.Transform(matrix));
        public Poly Translate(double dx, double dy) => CreatePoly(Points.Translate(dx, dy));

        public Rectangle BoundingBox()
        {
            if (_boundingBox != null)
                return _boundingBox;
            
            double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

            foreach (var p in Points)
            {
                x1 = Math.Min(x1, p.X);
                y1 = Math.Min(y1, p.Y);
                x2 = Math.Max(x2, p.X);
                y2 = Math.Max(y2, p.Y);
            }

            return (_boundingBox = new Rectangle(new Point(x1, y1), new Point(x2, y2)));
        }


        public Point ClosestPoint(Point p)
        {
            Point closest = null;

            double minDistance = double.MaxValue;

            foreach (var line in Segments)
            {
                Point closestPoint = line.ClosestPoint(p);

                double distance = p.DistanceTo(closestPoint);

                if (distance < minDistance)
                {
                    closest = closestPoint;
                    minDistance = distance;
                }
            }

            return closest;
        }

        public IEnumerable<LineSegment> Segments
        {
            get
            {
                for (int i=1;i<Points.Count;i++)
                    yield return new LineSegment(Points[i-1],Points[i]);

                if (Closed)
                    yield return new LineSegment(Points[Points.Count-1],Points[0]);
            }
        } 

        public Poly Simplify(double tolerance)
        {
            return CreatePoly(DouglasPeuckerReduction(Points, tolerance));
        }

        private static IEnumerable<Point> DouglasPeuckerReduction(IReadOnlyList<Point> points, double tolerance)
        {
            if (points == null || points.Count < 3)
                return points;

            int firstPoint = 0;
            int lastPoint = points.Count - 1;
            List<int> indexesToKeep = new List<int> { firstPoint, lastPoint };

            while (lastPoint >= firstPoint && points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            if (lastPoint > firstPoint)
                DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref indexesToKeep);

            return indexesToKeep.OrderBy(i => i).Select(i => points[i]);
        }

        private static void DouglasPeuckerReduction(IReadOnlyList<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        private static double PerpendicularDistance(Point point1, Point point2, Point point)
        {
            double area = Math.Abs(.5 * (point1.X * point2.Y + point2.X * point.Y + point.X * point1.Y - point2.X * point1.Y - point.X * point2.Y - point1.X * point.Y));
            double bottom = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));

            return area / bottom * 2;
        }

        public bool Intersects(Poly poly)
        {
            return Segments.Any(seg => poly.Segments.Any(seg.Intersects)) || (Closed && IsPointInside(poly.Points[0])) || (poly.Closed && poly.IsPointInside(this.Points[0]));
        }

        public IEnumerable<Point> Intersections(Poly other)
        {
            return Segments.SelectMany(seg => other.Segments.Select(s => s.Intersection(seg)).Where(point => point != null));
        }

        public bool Intersects(LineSegment segment)
        {
            return Segments.Any(seg => seg.Intersects(segment)) || (Closed && IsPointInside(segment.P1));
        }

        public IEnumerable<Point> Intersections(LineSegment other)
        {
            return Segments.Select(seg => seg.Intersection(other)).Where(pt => pt != null);
        }

        public bool IsPointInside(Point point)
        {
            if (!Closed)
                throw new NotSupportedException();

            int count = Points.Count;
            bool c = false;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if (((Points[i].Y >= point.Y) != (Points[j].Y >= point.Y)) && (point.X <= (Points[j].X - Points[i].X) * (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X))
                    c = !c;
            }

            return c;
        }

    }

}
