using System;

namespace Iridium.Geo
{
    public class LinearBezierCurve : BezierCurve
    {
        public LinearBezierCurve(Point p1, Point p2) : base(p1,p2)
        {
        }

        public override Point PointOnCurve(double t)
        {
            return new Point(P1.X + t*(P2.X-P1.X),P1.Y + t*(P2.Y-P1.Y));
        }

        public override int Order => 1;

        public override double Length => P1.DistanceTo(P2);
        public override double StartAngle => GeometryUtil.Angle(P1,P2);
        public override double EndAngle => GeometryUtil.Angle(P1,P2);

        public override Point ClosestPoint(Point p, out double distance)
        {
            var closest = new LineSegment(P1, P2).ClosestPoint(p);

            distance = p.DistanceTo(closest);

            return closest;
        }

        public override Point ClosestPoint(Point p)
        {
            return new LineSegment(P1,P2).ClosestPoint(p);
        }

        public override BezierCurve Rotate(double angle, Point origin = null)
        {
            return new LinearBezierCurve(P1.Rotate(angle,origin), P2.Rotate(angle,origin));
        }

        public override BezierCurve Translate(double dx, double dy)
        {
            return new LinearBezierCurve(P1.Translate(dx,dy), P2.Translate(dx,dy));
        }

        public override BezierCurve Scale(double factor, Point origin = null)
        {
            return new LinearBezierCurve(P1.Scale(factor, origin), P2.Scale(factor, origin));
        }

        public override BezierCurve Transform(AffineMatrix2D matrix)
        {
            return new LinearBezierCurve(P1.Transform(matrix), P2.Transform(matrix));
        }

        public override bool Intersects(LineSegment line)
        {
            return line.Intersects(new LineSegment(P1, P2));
        }

        public override Rectangle BoundingBox()
        {
            return new Rectangle(new Point(Math.Min(P1.X,P2.X), Math.Min(P1.Y,P2.Y)), new Point(Math.Max(P1.X,P2.X), Math.Max(P1.Y,P2.Y)));
        }
       }


}
