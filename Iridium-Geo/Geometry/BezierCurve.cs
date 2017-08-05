using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class BezierCurve : ICurve, ITransformable<BezierCurve>
    {
        private static double[][] _factorals = new[]
        {
            new double[] {1},
            new double[] {1,1},
            new double[] {1,2,1},
            new double[] {1,3,3,1},
            new double[] {1,4,6,4,1},
            new double[] {1,5,10,10,5,1},
            new double[] {1,6,15,20,15,6,1},
        };

        public IReadOnlyList<Point> Points;
        public int Order { get; }

        public BezierCurve(params Point[] points)
        {
            Points = points;
            Order = points.Length - 1;

            if (Order < 0 || Order >= _factorals.Length)
                throw new NotSupportedException("BezierCurve only support orders 0 to " + (_factorals.Length-1));
        }

        public Point PointOnCurve(double t)
        {
            double mt = 1 - t;

            switch (Order)
            {
                case 0:
                {
                    return Points[0];
                }

                case 1:
                    {
                        return new Point(
                            mt * Points[0].X + t * Points[2].X,
                            mt * Points[0].Y + t * Points[2].Y
                        );
                    }

                case 2:
                {
                        double t2 = t * t;
                        double mt2 = mt * mt;

                        return new Point(
                            mt2 * Points[0].X + 2 * t * mt * Points[1].X + t2 * Points[2].X,
                            mt2 * Points[0].Y + 2 * t * mt * Points[1].Y + t2 * Points[2].Y
                        );
                    }
                    
                case 3:
                {
                        double t2 = t * t;
                        double mt2 = mt * mt;

                        double t3 = t2 * t;
                        double mt3 = mt2 * mt;

                        return new Point(
                            mt3 * Points[0].X + 3 * mt2 * t * Points[1].X + 3 * mt * t2 * Points[2].X + t3 * Points[3].X,
                            mt3 * Points[0].Y + 3 * mt2 * t * Points[1].Y + 3 * mt * t2 * Points[2].Y + t3 * Points[3].Y
                        );
                    }
                    

                default:
                {
                    double x = 0;
                    double y = 0;
                    int n = Order;

                    for (int i = 0; i <= n; i++)
                    {
                        x += _factorals[n][i] * Math.Pow(mt, n - i) * Math.Pow(t, i) * Points[i].X;
                        y += _factorals[n][i] * Math.Pow(mt, n - i) * Math.Pow(t, i) * Points[i].Y;
                    }

                    return new Point(x,y);
                }
            }
        }

        public double Length => Partition().Length();
        public double StartAngle => StartPoint.AngleTo(Points[1]);
        public double EndAngle => Points[Points.Count - 2].AngleTo(EndPoint);


        public bool Intersects(LineSegment line) => Partition().Any(segment => segment.Intersects(line));

        public Point ClosestPoint(Point p)
        {
            Point closest = null;
            double distance = double.MaxValue;

            foreach (var tp in GeneratePoints(10))
            {
                var d = tp.DistanceTo(p);

                if (d < distance)
                {
                    distance = d;
                    closest = tp;
                }
            }

            return closest;
        }

	    public virtual IEnumerable<Point> GeneratePoints(int n)
	    {
            if (n < 3)
                throw new ArgumentException("need at least 3 points on bezier curve",nameof(n));

	        for (int i=0;i<n;i++)
                yield return PointOnCurve(i * 1.0/(n-1));
	    }


        public virtual IEnumerable<LineSegment> Partition(int segments = 10)
        {
            Point p = StartPoint;

            for (int i = 1; i <= segments; i++)
            {
                var p2 = PointOnCurve((double)i / segments);

                yield return new LineSegment(p, p2);

                p = p2;
            }
        }

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        public Rectangle BoundingBox() => Points.BoundingBox();

        public BezierCurve Rotate(double angle, Point origin = null) => new BezierCurve(Points.Rotate(angle,origin).ToArray());
        public BezierCurve Translate(double dx, double dy) => new BezierCurve(Points.Translate(dx,dy).ToArray());
        public BezierCurve Scale(double factor, Point origin = null) => new BezierCurve(Points.Scale(factor,origin).ToArray());
        public BezierCurve Transform(AffineMatrix2D matrix) => new BezierCurve(Points.Transform(matrix).ToArray());

        ILinearGeometry ITransformable<ILinearGeometry>.Transform(AffineMatrix2D matrix) => Transform(matrix);
        ILinearGeometry IScalable<ILinearGeometry>.Scale(double factor, Point origin) => Scale(factor, origin);
        ILinearGeometry IRotatable<ILinearGeometry>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ILinearGeometry ITranslatable<ILinearGeometry>.Translate(double dx, double dy) => Translate(dx, dy);

        ICurve ITransformable<ICurve>.Transform(AffineMatrix2D matrix) => Transform(matrix);
        ICurve IScalable<ICurve>.Scale(double factor, Point origin) => Scale(factor, origin);
        ICurve IRotatable<ICurve>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ICurve ITranslatable<ICurve>.Translate(double dx, double dy) => Translate(dx, dy);

        public Point StartPoint => Points[0];
        public Point EndPoint => Points[Points.Count-1];

        public static BezierCurve CreateSmallArc(Circle circle, double a1, double a2)
        {
            a1 = GeometryUtil.NormalizeAngle(a1);
            a2 = GeometryUtil.NormalizeAngle(a2);

            var r = circle.Radius;

            var a = (a2 - a1) / 2.0; // 
            var x4 = r * Math.Cos(a);
            var y4 = r * Math.Sin(a);
            var x1 = x4;
            var y1 = -y4;

            var k = 0.5522847498;
            var f = k * Math.Tan(a);

            var x2 = x1 + f * y4;
            var y2 = y1 + f * x4;
            var x3 = x2;
            var y3 = -y2;

            var ar = a + a1;
            var cos_ar = Math.Cos(ar);
            var sin_ar = Math.Sin(ar);

            return new BezierCurve(
                new Point(r * Math.Cos(a1), r * Math.Sin(a1)),
                new Point(x2 * cos_ar - y2 * sin_ar, x2 * sin_ar + y2 * cos_ar),
                new Point(x3 * cos_ar - y3 * sin_ar, x3 * sin_ar + y3 * cos_ar),
                new Point(r * Math.Cos(a2), r * Math.Sin(a2))
            ).Translate(circle.Center.X, circle.Center.Y);
        }

    }
}
