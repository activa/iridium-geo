using System;

namespace Iridium.Geo
{
    public class CubicBezierCurve : BezierCurve//<CubicBezierCurve>
    {
        public Point CP1 { get; }
        public Point CP2 { get; }

        public CubicBezierCurve(CubicBezierCurve c) : base(c.P1,c.P2)
        {
            CP1 = c.CP1;
            CP2 = c.CP2;
        }

        public CubicBezierCurve(Point p1, Point controlPoint1, Point controlPoint2, Point p2) : base(p1,p2)
        {
            CP1 = controlPoint1;
            CP2 = controlPoint2;
        }

        public override Point PointOnCurve(double t)
        {
            double t1 = 1 - t;

            return new Point(
                t1*t1*t1*P1.X + 3*t1*t1*t*CP1.X + 3*t1*t*t*CP2.X + t*t*t*P2.X,
                t1*t1*t1*P1.Y + 3*t1*t1*t*CP1.Y + 3*t1*t*t*CP2.Y + t*t*t*P2.Y
            );
            
        }

        public override int Order => 3;

        public override double StartAngle => GeometryUtil.Angle(P1, CP1);
        public override double EndAngle => GeometryUtil.Angle(CP2,P2);

        public override BezierCurve Rotate(double angle, Point origin = null)
        {
            return new CubicBezierCurve(P1.Rotate(angle,origin),CP1.Rotate(angle,origin),CP2.Rotate(angle,origin),P2.Rotate(angle,origin));
        }

        public override BezierCurve Translate(double dx, double dy)
        {
            return new CubicBezierCurve(P1.Translate(dx, dy), CP1.Translate(dx, dy), CP2.Translate(dx, dy), P2.Translate(dx, dy));
        }

        public override BezierCurve Scale(double factor, Point origin = null)
        {
            return new CubicBezierCurve(P1.Scale(factor, origin), CP1.Scale(factor, origin), CP2.Scale(factor, origin), P2.Scale(factor, origin));
        }

        public override BezierCurve Transform(AffineMatrix2D matrix)
        {
            return new CubicBezierCurve(P1.Transform(matrix), CP1.Transform(matrix), CP2.Transform(matrix), P2.Transform(matrix));
        }


        public static CubicBezierCurve CreateSmallArc(Circle circle, double a1, double a2)
        {
            // Compute all four points for an arc that subtends the same total angle
            // but is centered on the X-axis

            a1 = GeometryUtil.NormalizeAngle(a1);
            a2 = GeometryUtil.NormalizeAngle(a2);

            var r = circle.Radius;

            var a = (a2 - a1) / 2.0; // 
            var x4 = r * Math.Cos(a);
            var y4 = r * Math.Sin(a);
            var x1 = x4;
            var y1 = -y4;

            var k = 0.5522847498;
            var f = k * Math.Tan(a);

            var x2 = x1 + f * y4;
            var y2 = y1 + f * x4;
            var x3 = x2; 
            var y3 = -y2;

            // Find the arc points actual locations by computing x1,y1 and x4,y4 
            // and rotating the control points by a + a1

            var ar = a + a1;
            var cos_ar = Math.Cos(ar);
            var sin_ar = Math.Sin(ar);

            return (CubicBezierCurve)new CubicBezierCurve(
                new Point(r * Math.Cos(a1), r * Math.Sin(a1)), 
                new Point(x2 * cos_ar - y2 * sin_ar, x2 * sin_ar + y2 * cos_ar), 
                new Point(x3 * cos_ar - y3 * sin_ar, x3 * sin_ar + y3 * cos_ar), 
                new Point(r * Math.Cos(a2), r * Math.Sin(a2))
            ).Translate(circle.Center.X, circle.Center.Y);
        }

        public override Rectangle BoundingBox()
        {
            return new Rectangle(
                new Point(MathUtil.Min(P1.X,CP1.X,CP2.X,P2.X), MathUtil.Min(P1.Y,CP1.Y,CP2.Y,P2.Y)),
                new Point(MathUtil.Max(P1.X,CP1.X,CP2.X,P2.X), MathUtil.Max(P1.Y,CP1.Y,CP2.Y,P2.Y))
            );
           
        }

    }

}
