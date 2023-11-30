using System;
using System.Collections.Generic;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class GeoTest
    {
        private double[] _randomAngles;

        [OneTimeSetUp]
        public void Setup()
        {
            Random rnd = new Random();

            _randomAngles = new double[1000];

            for (int i = 0; i < 1000; i++)
                _randomAngles[i] = rnd.NextDouble();
        }


        [Test]
        public void TestSimplify()
        {
            Poly polygon = new Polyline(new[] { new Point(20,10), new Point(20, 10), new Point(20, 10), new Point(20, 10), new Point(20, 10), });

            var simplified = polygon.Simplify(0.1);

            Assert.That(simplified.Points.Count, Is.EqualTo(2));
            Assert.That(simplified.Points[0].X, Is.EqualTo(20));
            Assert.That(simplified.Points[0].Y, Is.EqualTo(10));
            Assert.That(simplified.Points[1].X, Is.EqualTo(20));
            Assert.That(simplified.Points[1].Y, Is.EqualTo(10));
        }

        [Test]
        public void TestSimplify2()
        {
            Poly polygon = new Polyline(new[] { new Point(20, 10), new Point(21, 15), new Point(22, 16), new Point(23, 10), new Point(24, 24), });

            var simplified = polygon.Simplify(0.000000001);

            Assert.That(simplified.Points.Count, Is.EqualTo(polygon.Points.Count));

            for (int i = 0; i < polygon.Points.Count; i++)
            {
                Assert.That(simplified.Points[i].X, Is.EqualTo(polygon.Points[i].X));
                Assert.That(simplified.Points[i].Y, Is.EqualTo(polygon.Points[i].Y));
            }
        }

        [Test]
        public void PolygonArea()
        {
            Rectangle rect = new Rectangle(new Point(5,5), new Point(20,10) );
            Polygon p = new Polygon(new[] {new Point(5, 5), new Point(20, 5), new Point(20, 10), new Point(5, 10),});

            Assert.That(rect.Area, Is.EqualTo(75).Within(0.00001));
            Assert.That(p.Area, Is.EqualTo(75).Within(0.00001));
        }



        [Test]
        public void NormalizeAngle()
        {
            for (int i = 0; i < _randomAngles.Length; i++)
                Assert.That(GeometryUtil.NormalizeAngle(_randomAngles[i]), Is.AtMost(Math.PI).And.AtLeast(-Math.PI));
        }

        [Test]
        [TestCaseSource(nameof(AnglesToNormalize))]
        public void NormalizeAngle2(double angle1, double angle2)
        {
            Assert.That(GeometryUtil.NormalizeAngle(angle1), Is.EqualTo(angle2).Within(0.00001));
        }

        public static IEnumerable<TestCaseData> AnglesToNormalize
        {
            get
            {
                yield return new TestCaseData(0.0,0.0);
                yield return new TestCaseData(1.0,1.0);
                yield return new TestCaseData(-1.0,-1.0);
                yield return new TestCaseData(Math.PI/2+1.0, Math.PI/2+1.0);
                yield return new TestCaseData(Math.PI + 1.0, -Math.PI+1.0);
            }
        }

        [Test]
        public void TotalDistanceBetweenPoints()
        {
            var points = new[]
            {
                new Point(10,5), 
                new Point(20,5), 
                new Point(20,10), 
                new Point(10,10), 
            };

            Assert.That(points.Length(true), Is.EqualTo(30));
            Assert.That(points.Length(false), Is.EqualTo(25));
        }

        [Test]
        public void SomeTest()
        {
            Point p = new Point(7, 6);
            Point aroundPoint = new Point(6,4);
            LineSegment seg = new LineSegment(new Point(3,6), new Point(8,2));

            AffineMatrix2D matrix = AffineMatrix2D.Factory.Mirror(seg);

            Point p2 = p.Transform(matrix);
        }
    }
}