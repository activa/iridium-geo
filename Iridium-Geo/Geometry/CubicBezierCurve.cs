using System;

namespace Iridium.Geo
{
    public class CubicBezierCurve : BezierCurve
    {
        private Point CP1 => ControlPoints[0];
        private Point CP2 => ControlPoints[1];

        public CubicBezierCurve(CubicBezierCurve c) : base(c.P1,c.P2, c.CP1, c.CP2)
        {
        }

        public CubicBezierCurve(Point p1, Point controlPoint1, Point controlPoint2, Point p2) : base(p1,p2,controlPoint1,controlPoint2)
        {
        }

        public override Point PointOnCurve(double t)
        {
            double mt = 1 - t;
            double t2 = t * t;
            double t3 = t2 * t;
            double mt2 = mt * mt;
            double mt3 = mt2 * mt;

            return new Point(
                mt3*P1.X + 3*mt2*t*CP1.X + 3*mt*t2*CP2.X + t3*P2.X,
                mt3*P1.Y + 3*mt2*t*CP1.Y + 3*mt*t2*CP2.Y + t3*P2.Y
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
