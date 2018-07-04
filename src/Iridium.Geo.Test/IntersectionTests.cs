using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iridium.Geo;
using NUnit.Framework;

namespace Iridium.Geo.Test
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

            Assert.That(c.Overlaps(pt), Is.False);
            Assert.That(pt.Overlaps(c), Is.False);

            c = new Circle(new Point(0, 0), 10);
            pt = new Point(10, 0);

            Assert.That(c.Overlaps(pt), Is.True);
            Assert.That(pt.Overlaps(c), Is.True);

            c = new Circle(new Point(0, 0), 10);
            pt = new Point(5, 0);

            Assert.That(c.Overlaps(pt), Is.True);
            Assert.That(pt.Overlaps(c), Is.True);
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
