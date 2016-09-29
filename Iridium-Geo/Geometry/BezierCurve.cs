using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public abstract class BezierCurve : IGeometry, ITransformable<BezierCurve>
    {
        public Point P1 { get; }
        public Point P2 { get; }

        protected BezierCurve(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public abstract Point PointOnCurve(double t);
        public abstract int Order {get;}
        public virtual double Length => Partition().Length();
        public abstract double StartAngle {get;}
        public abstract double EndAngle {get;}


        public virtual bool Intersects(LineSegment line) => Partition().Any(segment => segment.Intersects(line));

	    public virtual Point ClosestPoint(Point p)
	    {
	        double d;

	        return ClosestPoint(p, out d);
	    }

        public virtual Point ClosestPoint(Point p, out double distance)
        {
            Point closest = null;
            distance = double.MaxValue;

            foreach (var tp in Points(10))
            {
                double d = tp.DistanceTo(p);

                if (d < distance)
                {
                    distance = d;
                    closest = tp;
                }
            }

            return closest;
        }

	    public virtual IEnumerable<Point> Points(int n)
	    {
            if (n < 3)
                throw new ArgumentException("need at least 3 points on bezier curve",nameof(n));

	        for (int i=0;i<n;i++)
                yield return PointOnCurve(i * 1.0/(n-1));
	    }


        public virtual IEnumerable<LineSegment> Partition(int segments = 10)
        {
            Point p = P1;

            for (int i = 1; i <= segments; i++)
            {
                var p2 = PointOnCurve((double)i / segments);

                yield return new LineSegment(p, p2);

                p = p2;
            }
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
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


        public abstract Rectangle BoundingBox();

        public abstract BezierCurve Rotate(double angle, Point p = null);
        public abstract BezierCurve Translate(double dx, double dy);
        public abstract BezierCurve Scale(double factor, Point origin = null);
        public abstract BezierCurve Transform(AffineMatrix2D matrix);
    }
}
