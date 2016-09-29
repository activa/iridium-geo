namespace Iridium.Geo
{

    public class PointBezierCurve : BezierCurve
    {
        private readonly double _angle;
        
        public PointBezierCurve(Point p, double angle) : base(p,p)
        {
            _angle = angle;
        }

        public override Point PointOnCurve(double t)
        {
            return P1;
        }

        public override BezierCurve Rotate(double angle, Point p = null)
        {
            return new PointBezierCurve(p.Rotate(angle,p), _angle + angle); 
            
        }
        public override BezierCurve Translate(double dx, double dy)
        {
            return new PointBezierCurve(P1.Translate(dx,dy), _angle);
        }

        public override BezierCurve Scale(double factor, Point origin = null)
        {
            return new PointBezierCurve(P1.Scale(factor,origin), _angle);
        }

        public override BezierCurve Transform(AffineMatrix2D matrix)
        {
            return new PointBezierCurve(P1.Transform(matrix), _angle);
        }

        public override int Order => 0;

        public override double Length => 0.0;

        public override double StartAngle => _angle;
        public override double EndAngle => _angle;

        public override bool Intersects(LineSegment line)
        {
            return false;
        }

        public override Point ClosestPoint(Point p, out double distance)
        {
            distance = p.DistanceTo(P1);

            return P1;
        }

        public override Rectangle BoundingBox()
        {
            return new Rectangle(P1,P1);
        }
    }

}
