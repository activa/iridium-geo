using System;
using System.Linq;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class BezierTests
    {
        [Test]
        public void ArcToBezier()
        {
            Circle circle = new Circle(new Point(0, 0), 10);

            var bezier = BezierCurve.FromArc(circle, 0, Math.PI / 2);

            for (double t = 0.0; t <= 1.0; t += 0.001)
            {
                Point p = bezier.PointOnCurve(t);

                Assert.That(p.DistanceTo(Point.Zero), Is.EqualTo(10).Within(0.1).Percent);
                Assert.That(circle.Center.AngleTo(p), Is.InRange(0, Math.PI / 2));

            }
        }

        private Random _rnd = new Random();

        Point randomPoint() => new Point(_rnd.NextDouble() * 10, _rnd.NextDouble() * 10);

        BezierCurve randomBezier(int order = -1)
        {
            if (order < 0)
                order = _rnd.Next(5);

            var points = new Point[order + 1];

            for (int i = 0; i < order + 1; i++)
                points[i] = randomPoint();

            return new BezierCurve(points);
        }

        private bool PointsEqual(Point p1, Point p2) => Math.Abs(p1.X - p2.X) < 0.001 && Math.Abs(p1.Y - p2.Y) < 0.001;

        [Test]
        [Repeat(100)]
        public void SplitBezier()
        {
            Random rnd = new Random();

            BezierCurve curve = randomBezier();

            double t = rnd.NextDouble();

            var splitPoint = curve.PointOnCurve(t);

            var subCurves = curve.Split(t);

            Assert.That(subCurves.Item1.StartPoint.X, Is.EqualTo(curve.StartPoint.X).Within(0.001));
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
                    Assert.That(PointsEqual(curve.PointOnCurve(t2), subCurves.Item2.PointOnCurve(((t2 - t) / (1 - t)))));
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