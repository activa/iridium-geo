namespace Iridium.Geo
{
    public class QuadraticBezierCurve : BezierCurve
    {
        public Point CP { get; }

        public QuadraticBezierCurve(Point p1, Point controlPoint, Point p2) : base(p1,p2)
        {
            CP = controlPoint;
        }

        public override Point PointOnCurve(double t)
        {
            var t1 = 1-t;

            return new Point
                (
                    t1*t1*P1.X + 2*t*t1*CP.X + t*t*P2.X,
                    t1*t1*P1.Y + 2*t*t1*CP.Y + t*t*P2.Y
                );
        }

        public override BezierCurve Rotate(double angle, Point origin = null)
		{
		    return new QuadraticBezierCurve(P1.Rotate(angle, origin), CP.Rotate(angle, origin), P2.Rotate(angle, origin));
		}

		public override BezierCurve Translate(double dx, double dy)
		{
		    return new QuadraticBezierCurve(P1.Translate(dx, dy), CP.Translate(dx, dy), P2.Translate(dx, dy));
		}

		public override BezierCurve Scale(double factor, Point origin = null)
		{
		    return new QuadraticBezierCurve(P1.Scale(factor, origin), CP.Scale(factor, origin), P2.Scale(factor, origin));
		}

        public override BezierCurve Transform(AffineMatrix2D matrix)
        {
            return new QuadraticBezierCurve(P1.Transform(matrix), CP.Transform(matrix), P2.Transform(matrix));
        }

        public override Rectangle BoundingBox()
        {
            return new Rectangle(
                new Point(MathUtil.Min(P1.X, CP.X, P2.X), MathUtil.Min(P1.Y, CP.Y, P2.Y)),
                new Point(MathUtil.Max(P1.X, CP.X, P2.X), MathUtil.Max(P1.Y, CP.Y, P2.Y))
                );
        }

        public override int Order => 2;

		public override double StartAngle => GeometryUtil.Angle(P1, CP);
		public override double EndAngle => GeometryUtil.Angle(CP, P2);
	}

}
