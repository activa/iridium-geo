using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class BezierCurve : ICurve, ITransformable<BezierCurve>, IIntersectable<LineSegment>, IIntersectable<BezierCurve>
    {
        private static readonly double[][] _factorals = {
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

        public BezierCurve(IEnumerable<Point> points) : this(points.ToArray())
        {
        }

        public BezierCurve(LineSegment lineSegment) : this(lineSegment.StartPoint, lineSegment.EndPoint)
        {
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
                            mt * Points[0].X + t * Points[1].X,
                            mt * Points[0].Y + t * Points[1].Y
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

        public bool Intersects(LineSegment line) => Intersects(new BezierCurve(line));
        public IEnumerable<Point> Intersections(LineSegment line) => Intersections(new BezierCurve(line));

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

        public static BezierCurve FromArc(Circle circle, double startAngle, double endAngle)
        {
            startAngle = GeometryUtil.NormalizeAngle(startAngle);
            endAngle = GeometryUtil.NormalizeAngle(endAngle);

            var r = circle.Radius;

            var a = (endAngle - startAngle) / 2.0; // 
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

            var ar = a + startAngle;
            var cos_ar = Math.Cos(ar);
            var sin_ar = Math.Sin(ar);

            return new BezierCurve(
                new Point(r * Math.Cos(startAngle), r * Math.Sin(startAngle)),
                new Point(x2 * cos_ar - y2 * sin_ar, x2 * sin_ar + y2 * cos_ar),
                new Point(x3 * cos_ar - y3 * sin_ar, x3 * sin_ar + y3 * cos_ar),
                new Point(r * Math.Cos(endAngle), r * Math.Sin(endAngle))
            ).Translate(circle.Center.X, circle.Center.Y);
        }

        public  bool Intersects(BezierCurve c2)
        {
            var bbox1 = this.BoundingBox();
            var bbox2 = c2.BoundingBox();

            if (!bbox1.Intersects(bbox2))
                return false;

            if (bbox1.Area + bbox2.Area < 0.01)
                return true;

            var (c1a, c1b) = this.Split(0.5);
            var (c2a, c2b) = c2.Split(0.5);

            return c1a.Intersects(c2a) || c1a.Intersects(c2b) || c1b.Intersects(c2a) || c1b.Intersects(c2b);
        }

        public IEnumerable<Point> Intersections(BezierCurve other)
        {
            List<Point> points = new List<Point>();

            void checkIntersections(BezierCurve c1, BezierCurve c2)
            {
                var bbox1 = c1.BoundingBox();
                var bbox2 = c2.BoundingBox();

                if (!bbox1.Intersects(bbox2))
                    return;

                if (bbox1.Area + bbox2.Area < 0.1)
                {
                    var pt = new LineSegment(c1.StartPoint, c1.EndPoint).Intersection(new LineSegment(c2.StartPoint, c2.EndPoint));

                    if (pt != null)
                        points.Add(pt);

                    return;
                }

                var (c1a, c1b) = c1.Split(0.5);
                var (c2a, c2b) = c2.Split(0.5);

                checkIntersections(c1a,c2a);
                checkIntersections(c1a,c2b);
                checkIntersections(c1b,c2a);
                checkIntersections(c1b,c2b);
            }

            checkIntersections(this,other);

            return points;
        }

        public (BezierCurve,BezierCurve) Split(double at)
        {
            List<Point> left = new List<Point>();
            List<Point> right = new List<Point>();

            void splitCurve(Point[] points, double t)
            {
                if (points.Length == 1)
                {
                    left.Add(points[0]);
                    right.Insert(0,points[0]);
                    return;
                }

                Point[] newPoints = new Point[points.Length-1];

                for (int i = 0; i < newPoints.Length; i++)
                {
                    if (i == 0)
                        left.Add(points[i]);
                    if (i == newPoints.Length - 1)
                        right.Insert(0,points[i+1]);

                    newPoints[i] = new Point((1 - t) * points[i].X + t * points[i + 1].X, (1 - t) * points[i].Y + t * points[i + 1].Y);
                }

                splitCurve(newPoints, t);
            }

            splitCurve(Points.ToArray(), at);

            return (
                new BezierCurve(left), 
                new BezierCurve(right)
            );
        }

        public bool Intersects(ILinearGeometry other)
        {
            switch (other)
            {
                case BezierCurve c: return Intersects(c);
                case LineSegment seg: return Intersects(seg);

                default: throw new NotImplementedException();
            }
        }

        public IEnumerable<Point> Intersections(ILinearGeometry other)
        {
            switch (other)
            {
                case BezierCurve c: return Intersections(c);
                case LineSegment seg: return Intersections(seg);

                default: throw new NotImplementedException();
            }
        }
    }
}
