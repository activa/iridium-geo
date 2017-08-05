using System;

namespace Iridium.Geo
{
	public class LineSegment : ILinearGeometry, ITransformable<LineSegment>, IIntersectable<LineSegment>
    {
        private double? _angle;
        private double? _length;

        public Point P1 { get; }
	    public Point P2 { get; }

	    public LineSegment(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public LineSegment(Point p1, double length, double angle)
        {
            _length = length;
            _angle = angle;

            P1 = p1;
            P2 = new Point(p1,angle,length);
        }

        public double Angle => (double) (_angle ?? (_angle = Math.Atan2(P2.Y-P1.Y,P2.X-P1.X)));
        public double Length => (double) (_length ?? (_length = P1.DistanceTo(P2)));
        double ILinearGeometry.StartAngle => Angle;
        double ILinearGeometry.EndAngle => Angle;
        public Point StartPoint => P1;
        public Point EndPoint => P2;
        public Point Center => new Point(P1.X + (P2.X-P1.X)/2, P1.Y + (P2.Y-P1.Y)/2);

        public Rectangle BoundingBox()
        {
            return new Rectangle(new Point(Math.Min(P1.X,P2.X), Math.Min(P1.Y,P2.Y)), new Point(Math.Max(P1.X,P2.X),Math.Max(P1.Y,P2.Y)));
        }


        public Point Intersection(LineSegment line2)
        {
            double p1X = P1.X;
            double p1Y = P1.Y;
            double p2X = P2.X;
            double p2Y = P2.Y;

            double p3X = line2.P1.X;
            double p3Y = line2.P1.Y;
            double p4X = line2.P2.X;
            double p4Y = line2.P2.Y;

            double s1X = p2X - p1X;
            double s1Y = p2Y - p1Y;
            double s2X = p4X - p3X;
            double s2Y = p4Y - p3Y;

            double s = (-s1Y * (p1X - p3X) + s1X * (p1Y - p3Y)) / (-s2X * s1Y + s1X * s2Y);
            double t = (s2X * (p1Y - p3Y) - s2Y * (p1X - p3X)) / (-s2X * s1Y + s1X * s2Y);

            if (s >= 0.0 && s <= 1.0 && t >= 0.0 && t <= 1.0)
            {
                return new Point(p1X + (t * s1X), p1Y + (t * s1Y));
            }

            return null; // No collision
        }

        public bool Intersects(LineSegment line2)
        {
            double p1X = P1.X;
            double p1Y = P1.Y;
            double p2X = P2.X;
            double p2Y = P2.Y;

            double p3X = line2.P1.X;
            double p3Y = line2.P1.Y;
            double p4X = line2.P2.X;
            double p4Y = line2.P2.Y;

            double s1X = p2X - p1X;
            double s1Y = p2Y - p1Y;
            double s2X = p4X - p3X;
            double s2Y = p4Y - p3Y;

            double s = (-s1Y * (p1X - p3X) + s1X * (p1Y - p3Y)) / (-s2X * s1Y + s1X * s2Y);
            double t = (s2X * (p1Y - p3Y) - s2Y * (p1X - p3X)) / (-s2X * s1Y + s1X * s2Y);

            return s >= 0.0 && s <= 1.0 && t >= 0.0 && t <= 1.0;
        }

        public Point ClosestPoint(Point point)
        {
            Vector vec1 = new Vector(point.X - P1.X, point.Y - P1.Y);
            Vector vec2 = new Vector(P2.X - P1.X, P2.Y - P1.Y);

            var len2 = MathUtil.Square(vec2.X) + MathUtil.Square(vec2.Y);

            if (len2 <= double.Epsilon)
                return P1;

            var t = (vec1.X*vec2.X + vec1.Y*vec2.Y)/len2;

            if (t <= 0.0) return P1;
            if (t >= 1.0) return P2;

            return new Point(P1.X + vec2.X * t, P1.Y + vec2.Y * t);
        }

	    public double DistanceTo(Point point) => ClosestPoint(point).DistanceTo(point);

        public LineSegment Translate(double dx, double dy) => new LineSegment(P1.Translate(dx, dy), P2.Translate(dx, dy));
        public LineSegment Rotate(double angle, Point origin = null) => new LineSegment(P1.Rotate(angle, origin), P2.Rotate(angle, origin));
        public LineSegment Scale(double factor, Point origin = null) => new LineSegment(P1.Scale(factor, origin), P2.Scale(factor, origin));
        public LineSegment Transform(AffineMatrix2D matrix) => new LineSegment(P1.Transform(matrix), P2.Transform(matrix));

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        ILinearGeometry IScalable<ILinearGeometry>.Scale(double factor, Point origin) => Scale(factor, origin);
        ILinearGeometry IRotatable<ILinearGeometry>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ILinearGeometry ITranslatable<ILinearGeometry>.Translate(double dx, double dy) => Translate(dx, dy);
        ILinearGeometry ITransformable<ILinearGeometry>.Transform(AffineMatrix2D matrix) => Transform(matrix);
    }
}
