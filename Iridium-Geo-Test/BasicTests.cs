using System;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium_Geo_Test
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

            var bezier = BezierCurve.CreateSmallArc(circle, 0, Math.PI / 2);

            for (double t = 0.0; t <= 1.0; t += 0.001)
            {
                Point p = bezier.PointOnCurve(t);

                Assert.That(p.DistanceTo(Point.Zero), Is.EqualTo(10).Within(0.1).Percent);
                Assert.That(circle.Center.AngleTo(p), Is.InRange(0,Math.PI/2));

            }
        }

    }
}