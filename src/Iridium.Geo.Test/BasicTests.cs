using System;
using System.Linq;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void Ellipse()
        {
            double rY = 5.0;
            double cord = Math.Sqrt(50.0) * 2;
            double rX = (cord - 10.0) / 2 + 5.0;

            Ellipse ellipse = new Ellipse(Point.Zero, rX, rY);

            Assert.That(ellipse.Cord, Is.EqualTo(cord));
            Assert.That(ellipse.CordSegment.Length, Is.EqualTo(10.0).Within(1).Ulps);

            ellipse = new Ellipse(new Point(-5, 0), new Point(5, 0), cord);

            Assert.That(ellipse.Center.X, Is.EqualTo(0.0).Within(1).Ulps);
            Assert.That(ellipse.Center.Y, Is.EqualTo(0.0).Within(1).Ulps);
            Assert.That(ellipse.RadiusX, Is.EqualTo(rX).Within(1).Ulps);
            Assert.That(ellipse.RadiusY, Is.EqualTo(rY).Within(1).Ulps);
        }


        [Test]
        public void EllipseBoundingBox()
        {
            Ellipse ellipse = new Ellipse(Point.Zero, 10, 5);

            var boundingBox = ellipse.BoundingBox();

            Assert.That(boundingBox.Width, Is.EqualTo(20.0));
            Assert.That(boundingBox.Height, Is.EqualTo(10.0));
            Assert.That(boundingBox.P1.X, Is.EqualTo(-10.0));
        }

        [Test]
        public void RotateEllipse90()
        {
            Ellipse ellipse = new Ellipse(Point.Zero, 10, 5);

            ellipse = ellipse.Rotate(Math.PI / 2);

            var boundingBox = ellipse.BoundingBox();

            Assert.That(boundingBox.Width, Is.EqualTo(10.0));
            Assert.That(boundingBox.Height, Is.EqualTo(20.0));
            Assert.That(boundingBox.P1.X, Is.EqualTo(-5.0));
        }

        [Test]
        public void PointRotate()
        {
            Point p = new Point(4, 3);

            Point p2 = new Point(-3, 4);

            var angle = Math.Atan2(4, -3);

            Point p2a = p.Transform(AffineMatrix2D.Factory.Rotate(Math.PI / 2));
            Point p2b = p.Rotate(Math.PI / 2);
            Point p2c = new Point(Point.Zero, angle, 5);

            Assert.That(p2a.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y).Within(0.001));
            Assert.That(p2b.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y).Within(0.001));
            Assert.That(p2c.X, Is.EqualTo(p2.X).Within(0.001));
            Assert.That(p2c.Y, Is.EqualTo(p2.Y).Within(0.001));
        }

        [Test]
        public void MirrorAroundPoint()
        {
            Point p = new Point(4, 3);
            Point p2 = new Point(-2, -1);

            Point p2a = p.Transform(AffineMatrix2D.Factory.Translate(-1, -1).Rotate(Math.PI).Translate(1, 1));
            Point p2b = p.Mirror(new Point(1, 1));

            Assert.That(p2b.X, Is.EqualTo(p2.X).Within(0.000001));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y).Within(0.000001));

            Assert.That(p2a.X, Is.EqualTo(p2.X).Within(0.000001));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y).Within(0.000001));
        }

        [Test]
        public void MatrixTranslate()
        {
            Point p = new Point(4, 3);
            Point p2 = new Point(6, 4);

            Point p2a = p.Translate(2, 1);
            Point p2b = p.Transform(AffineMatrix2D.Factory.Translate(2, 1));
            Point p2c = p.Transform(AffineMatrix2D.Identity.Translate(2, 1));

            Assert.That(p2a.X, Is.EqualTo(p2.X));
            Assert.That(p2a.Y, Is.EqualTo(p2.Y));
            Assert.That(p2b.X, Is.EqualTo(p2.X));
            Assert.That(p2b.Y, Is.EqualTo(p2.Y));
            Assert.That(p2c.X, Is.EqualTo(p2.X));
            Assert.That(p2c.Y, Is.EqualTo(p2.Y));

        }

        [Test]
        public void LineSegmentAngle()
        {
            LineSegment seg = new LineSegment(new Point(0, 0), new Point(0, 1));

            Assert.That(seg.Angle, Is.EqualTo(Math.PI / 2));
        }

        [Test]
        public void ArcToBezier()
        {
            Circle circle = new Circle(new Point(0, 0), 10);

            var bezier = BezierCurve.FromArc(circle, 0, Math.PI / 2);

            for (double t = 0.0; t <= 1.0; t += 0.001)
            {
                Point p = bezier.PointOnCurve(t);

                Assert.That(p.DistanceTo(Point.Zero), Is.EqualTo(10).Within(0.1).Percent);
                Assert.That(circle.Center.AngleTo(p), Is.InRange(0,Math.PI/2));

            }
        }

        private Random _rnd = new Random();

        Point randomPoint() => new Point(_rnd.NextDouble() * 10, _rnd.NextDouble() * 10);

        BezierCurve randomBezier(int order = -1)
        {
            if (order < 0)
                order = _rnd.Next(5);

            var points = new Point[order+1];

            for (int i = 0; i < order+1; i++)
                points[i] = randomPoint();

            return new BezierCurve(points);
        }

        private bool PointsEqual(Point p1, Point p2) => Math.Abs(p1.X-p2.X) < 0.001 && Math.Abs(p1.Y-p2.Y) < 0.001;

        [Test][Repeat(1000)]
        public void SplitBezier()
        {
            Random rnd = new Random();

            BezierCurve curve = randomBezier();

            double t = rnd.NextDouble();

            var splitPoint = curve.PointOnCurve(t);

            var subCurves = curve.Split(t);

            Assert.That(subCurves.Item1.StartPoint.X , Is.EqualTo(curve.StartPoint.X).Within(0.001));
            Assert.That(subCurves.Item1.StartPoint.Y, Is.EqualTo(curve.StartPoint.Y).Within(0.001));

            Assert.That(subCurves.Item1.EndPoint.X, Is.EqualTo(splitPoint.X).Within(0.001));
            Assert.That(subCurves.Item1.EndPoint.Y, Is.EqualTo(splitPoint.Y).Within(0.001));
            Assert.That(subCurves.Item2.StartPoint.X, Is.EqualTo(splitPoint.X).Within(0.001));
            Assert.That(subCurves.Item2.StartPoint.Y, Is.EqualTo(splitPoint.Y).Within(0.001));

            Assert.That(subCurves.Item2.EndPoint.X, Is.EqualTo(curve.EndPoint.X).Within(0.001));
            Assert.That(subCurves.Item2.EndPoint.Y, Is.EqualTo(curve.EndPoint.Y).Within(0.001));

            for (var t2 = 0.0; t2 <= 1.0; t2 += 0.1)
            {
                if (t2 <= t && t > 0.0)
                {
                    Assert.That(PointsEqual(curve.PointOnCurve(t2), subCurves.Item1.PointOnCurve(t2 / t)));
                }

                if (t2 >= t && t < 1.0)
                {
                    Assert.That(PointsEqual(curve.PointOnCurve(t2), subCurves.Item2.PointOnCurve(((t2-t) / (1-t)))));
                }
            }

        }

        [Test]
        public void BezierIntersection()
        {
            var beziers1 = Enumerable.Range(0, 100).Select(_ => randomBezier());
            var beziers2 = Enumerable.Range(0, 100).Select(_ => randomBezier());

            var intersections = (from b1 in beziers1 from b2 in beziers2 select b1.Intersections(b2).ToArray());

            int n = intersections.Count();
        }

    }
}