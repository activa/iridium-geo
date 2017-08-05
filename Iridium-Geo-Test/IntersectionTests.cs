using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium_Geo_Test
{
    [TestFixture]
    public class IntersectionTests
    {
        [Test]
        public void TestIntersectionCircles()
        {
            Circle c1 = new Circle(new Point(0, 0), 10);
            Circle c2 = new Circle(new Point(20, 0), 5);

            Assert.That(c1.Intersects(c2), Is.False);

            c1 = new Circle(new Point(0, 0), 10);
            c2 = new Circle(new Point(15, 0), 5);

            Assert.That(c1.Intersects(c2), Is.True);

            c1 = new Circle(new Point(0, 0), 10);
            c2 = new Circle(new Point(10, 0), 5);

            Assert.That(c1.Intersects(c2), Is.True);
        }

        [Test]
        public void TestIntersectionCirclePoint()
        {
            Circle c = new Circle(new Point(0, 0), 10);
            Point pt = new Point(20, 0);

            Assert.That(c.Intersects(pt), Is.False);
            Assert.That(pt.Intersects(c), Is.False);

            c = new Circle(new Point(0, 0), 10);
            pt = new Point(10, 0);

            Assert.That(c.Intersects(pt), Is.True);
            Assert.That(pt.Intersects(c), Is.True);

            c = new Circle(new Point(0, 0), 10);
            pt = new Point(5, 0);

            Assert.That(c.Intersects(pt), Is.True);
            Assert.That(pt.Intersects(c), Is.True);
        }

        [Test]
        public void TestUnsupportedIntersection()
        {
            Assert.Catch<NotImplementedException>(() =>
            {
                new Circle(new Point(0, 0), 1).Intersects(new Polygon(new Point[] { new Point(0, 0), }));
            });
        }

    }
}
