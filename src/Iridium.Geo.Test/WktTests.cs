using System.Collections.Generic;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class WktTests
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

            Assert.That(geo, Is.TypeOf<MultiPoly>());

            var multiPolygon = (MultiPoly)geo;

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
            yield return new TestCaseData(new MultiPoly(new [] {new Polyline(new [] { new Point(5,6), new Point(19,20) }), new Polyline(new[] { new Point(1, 2), new Point(9, 10) }) }), "MULTILINESTRING ((5 6,19 20),(1 2,9 10))");

        }

    }
}