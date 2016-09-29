using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class Polygon : IGeometry, ITransformable<Polygon>
    {
        public IReadOnlyList<Point> Points { get; }
        public bool Closed { get; }

        private Rectangle _boundingBox;

        public Polygon(IEnumerable<Point> points, bool closed = false)
        {
            Points = points.ToArray();
            Closed = closed;
        }

        public bool IsPointInside(Point point)
        {
            int count = Points.Count;
            bool c = false;

            for (int i = 0, j = count - 1; i < count; j = i++) 
            {
                if (((Points[i].Y >= point.Y ) != (Points[j].Y >= point.Y) ) && (point.X <= (Points[j].X - Points[i].X) * (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X))
                    c = !c;
            }

            return c;
        }

        public Polygon Translate(double dx, double dy)
        {
            return new Polygon(Points.Translate(dx,dy));
        }

        public Polygon Rotate(double angle, Point origin = null)
        {
            return new Polygon(Points.Rotate(angle, origin));
        }

		public Polygon Scale(double factor, Point origin = null)
		{
			return new Polygon(Points.Scale(factor, origin));
		}

        public Polygon Transform(AffineMatrix2D matrix)
        {
            return new Polygon(Points.Transform(matrix));
        }

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

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }


        public Point ClosestPoint(Point p)
        {
            Point closest = null;
            double minDistance = double.MaxValue;

            Point p1 = Points[Points.Count-1];

            foreach (Point p2 in Points)
            {
                LineSegment line = new LineSegment(p1,p2);

                Point closestPoint = line.ClosestPoint(p);

                double distance = p.DistanceTo(closestPoint);

                if (distance < minDistance)
                {
                    closest = closestPoint;
                    minDistance = distance;
                }
                p1 = p2;

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

        public Point Intersection(LineSegment ray)
        {
            double smallestDistance = double.MaxValue;
            Point intersectionPoint = null;

            foreach (var lineSegment in Segments)
            {
                Point p = lineSegment.Intersection(ray);

                if (p != null)
                {
                    double distance = ray.P1.DistanceTo(p);

                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        intersectionPoint = p;
                    }
                }
                

            }

            return intersectionPoint;
        }

        public Polygon Simplify(double tolerance)
        {
            return new Polygon(DouglasPeuckerReduction(Points, tolerance), Closed);
        }

        private static IEnumerable<Point> DouglasPeuckerReduction(IReadOnlyList<Point> points, double tolerance)
        {
            if (points == null || points.Count < 3)
                return points;

            int firstPoint = 0;
            int lastPoint = points.Count - 1;
            List<int> pointIndexsToKeep = new List<int> { firstPoint, lastPoint };

            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            return pointIndexsToKeep.OrderBy(i => i).Select(i => points[i]);
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

        public static double PerpendicularDistance(Point point1, Point point2, Point point)
        {
            double area = Math.Abs(.5 * (point1.X * point2.Y + point2.X * point.Y + point.X * point1.Y - point2.X * point1.Y - point.X * point2.Y - point1.X * point.Y));
            double bottom = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));

            return area / bottom * 2;
        }

    }
}



