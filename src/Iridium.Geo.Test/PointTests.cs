using System;
using NUnit.Framework;

namespace Iridium.Geo.Test
{
    [TestFixture]
    public class PointTests
    {
        [Test]
        public void AddVector()
        {
            Point p = new Point(1,2);

            Point p2 = p + new Vector(10, 20);

            Assert.That(p2.X, Is.EqualTo(11));
            Assert.That(p2.Y, Is.EqualTo(22));
        }

        [Test]
        public void AngleTo()
        {
            Point p1 = (0,0);
            Point p2 = (2,0);

            Assert.That(p1.AngleTo(p2), Is.EqualTo(0).Within(1).Ulps);

            p2 = (0,1);

            Assert.That(p1.AngleTo(p2), Is.EqualTo(Math.PI/2).Within(1).Ulps);


            p2 = (0,-1);
           
            Assert.That(p1.AngleTo(p2), Is.EqualTo(-Math.PI/2).Within(1).Ulps);
        }

        [Test]
        public void PointFromAngle()
        {
            Point p1 = new Point(0,0);
            Point p2 = new Point(p1,0.0,1.0);

            Assert.That(p2.X, Is.EqualTo(1).Within(0.00001));
            Assert.That(p2.Y, Is.EqualTo(0).Within(0.00001));

            p2 = new Point(p1, Math.PI/2, 1.0);

            Assert.That(p2.X, Is.EqualTo(0).Within(0.00001));
            Assert.That(p2.Y, Is.EqualTo(1).Within(0.00001));
        }

    }
}