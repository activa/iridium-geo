using System;

namespace Iridium.Geo
{
    public class Arc
    {
        public Circle Circle { get; }
        public double StartAngle { get; }
        public double EndAngle { get; }
        public bool Increasing { get; }

        public Arc(Circle circle, double startAngle, double endAngle, bool clockWise)
        {
            Circle = circle;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Increasing = clockWise;
        }

        public double Length => GeometryUtil.ArcLength(Circle.Radius, Increasing ? GeometryUtil.NormalizeAngle(EndAngle - StartAngle) : GeometryUtil.NormalizeAngle(StartAngle - EndAngle));

        public double EndPointDirectionAngle => Increasing ? GeometryUtil.NormalizeAngle(EndAngle + Math.PI / 2) : GeometryUtil.NormalizeAngle(EndAngle - Math.PI/2);

        public Arc Translate(double dx, double dy)
        {
            return new Arc(Circle.Translate(dx, dy), StartAngle, EndAngle, Increasing);
        }

        public Arc Rotate(double angle, Point origin)
        {
            return new Arc(
                new Circle(Circle.Center.Rotate(angle, origin), Circle.Radius),
                StartAngle + angle,
                EndAngle + angle,
                Increasing);
        }
    }

}
