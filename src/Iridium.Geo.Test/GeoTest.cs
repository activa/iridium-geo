using System;
using System.Collections.Generic;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class GeoTest
    {
        [Test]
        public void WKT_Point_Int()
        {
            var wkt = "POINT(5 6)";

            var geo = WKTParser.Parse(wkt);

            Assert.That(geo,Is.TypeOf<Point>());

            var pt = (Point)geo;

            Assert.That(pt.X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(pt.Y, Is.EqualTo(6).Within(1).Ulps);
        }

        [Test]
        public void WKT_Point_Float()
        {
            var geo = WKTParser.Parse("POINT(5.0 6.0)");

            Assert.That(geo, Is.TypeOf<Point>());

            var pt = (Point)geo;

            Assert.That(pt.X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(pt.Y, Is.EqualTo(6).Within(1).Ulps);
        }

        [Test]
        public void WKT_LineString_Int()
        {
            var geo = WKTParser.Parse("LINESTRING(5 6 , 19 20)");

            Assert.That(geo, Is.TypeOf<Polyline>());

            var polygon = (Polyline)geo;

            Assert.That(polygon.Points.Count, Is.EqualTo(2));

            Assert.That(polygon.Points[0].X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(polygon.Points[0].Y, Is.EqualTo(6).Within(1).Ulps);
            Assert.That(polygon.Points[1].X, Is.EqualTo(19).Within(1).Ulps);
            Assert.That(polygon.Points[1].Y, Is.EqualTo(20).Within(1).Ulps);
        }

        [Test]
        public void WKT_MultiLineString_Int()
        {
            var geo = WKTParser.Parse("MULTILINESTRING((5 6 , 19 20),(1 2, 9 10))");

            Assert.That(geo, Is.TypeOf<MultiPolyline>());

            var multiPolygon = (MultiPolyline)geo;

            Assert.That(multiPolygon.Polylines.Count, Is.EqualTo(2));

            Assert.That(multiPolygon.Polylines[0].Points[0].X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[0].Points[0].Y, Is.EqualTo(6).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[0].Points[1].X, Is.EqualTo(19).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[0].Points[1].Y, Is.EqualTo(20).Within(1).Ulps);

            Assert.That(multiPolygon.Polylines[1].Points[0].X, Is.EqualTo(1).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[1].Points[0].Y, Is.EqualTo(2).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[1].Points[1].X, Is.EqualTo(9).Within(1).Ulps);
            Assert.That(multiPolygon.Polylines[1].Points[1].Y, Is.EqualTo(10).Within(1).Ulps);
        }

        [Test]
        public void WKT_MultiPoint_Int_Style1()
        {
            var geo = WKTParser.Parse("MULTIPOINT(5 6 , 19 20 , 1 2, 9 10)");

            Assert.That(geo, Is.TypeOf<MultiPoint>());

            var multiPoint = (MultiPoint)geo;

            Assert.That(multiPoint.Points.Count, Is.EqualTo(4));

            Assert.That(multiPoint.Points[0].X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(multiPoint.Points[0].Y, Is.EqualTo(6).Within(1).Ulps);
            Assert.That(multiPoint.Points[1].X, Is.EqualTo(19).Within(1).Ulps);
            Assert.That(multiPoint.Points[1].Y, Is.EqualTo(20).Within(1).Ulps);

            Assert.That(multiPoint.Points[2].X, Is.EqualTo(1).Within(1).Ulps);
            Assert.That(multiPoint.Points[2].Y, Is.EqualTo(2).Within(1).Ulps);
            Assert.That(multiPoint.Points[3].X, Is.EqualTo(9).Within(1).Ulps);
            Assert.That(multiPoint.Points[3].Y, Is.EqualTo(10).Within(1).Ulps);
        }

        [Test]
        public void WKT_MultiPoint_Int_Style2()
        {
            var geo = WKTParser.Parse("MULTIPOINT((5 6) , (19 20) , (1 2), (9 10))");

            Assert.That(geo, Is.TypeOf<MultiPoint>());

            var multiPoint = (MultiPoint)geo;

            Assert.That(multiPoint.Points.Count, Is.EqualTo(4));

            Assert.That(multiPoint.Points[0].X, Is.EqualTo(5).Within(1).Ulps);
            Assert.That(multiPoint.Points[0].Y, Is.EqualTo(6).Within(1).Ulps);
            Assert.That(multiPoint.Points[1].X, Is.EqualTo(19).Within(1).Ulps);
            Assert.That(multiPoint.Points[1].Y, Is.EqualTo(20).Within(1).Ulps);

            Assert.That(multiPoint.Points[2].X, Is.EqualTo(1).Within(1).Ulps);
            Assert.That(multiPoint.Points[2].Y, Is.EqualTo(2).Within(1).Ulps);
            Assert.That(multiPoint.Points[3].X, Is.EqualTo(9).Within(1).Ulps);
            Assert.That(multiPoint.Points[3].Y, Is.EqualTo(10).Within(1).Ulps);
        }


        [TestCaseSource(nameof(WKT_TestData))]
        public void ToWKT(IGeometry geometry, string expectedWKT)
        {
            var generatedWKT = WKTWriter.ToWKT(geometry);

            Assert.That(generatedWKT, Is.EqualTo(expectedWKT));
        }

        public static IEnumerable<TestCaseData> WKT_TestData()
        {
            yield return new TestCaseData(new Point(1,2),"POINT (1 2)");
            yield return new TestCaseData(new MultiPoint(new[] { new Point(1,2), new Point(9,10),  }), "MULTIPOINT (1 2,9 10)");
            yield return new TestCaseData(new Polyline(new[] { new Point(1, 2), new Point(9, 10), }), "LINESTRING (1 2,9 10)");
            yield return new TestCaseData(new MultiPolyline(new [] {new Polyline(new [] { new Point(5,6), new Point(19,20) }), new Polyline(new[] { new Point(1, 2), new Point(9, 10) }) }), "MULTILINESTRING ((5 6,19 20),(1 2,9 10))");

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
            Polygon p = rect;

            Assert.That(rect.Area, Is.EqualTo(75));
            Assert.That(p.Area, Is.EqualTo(75));
        }


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
            var points = new Point[]
            {
                new Point(10,5), new Point(20,5), new Point(20,10), new Point(10,10), 
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