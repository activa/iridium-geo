using System;

namespace Iridium.Geo
{
    public sealed class Point : IGeometry, ITransformable<Point>
    {
        public static readonly Point Zero = new Point(0, 0);

        public double X { get; }
        public double Y { get; }

        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point(Point p, double angle, double distance)
        {
            X = p.X + distance * Math.Cos(angle);
            Y = p.Y + distance * Math.Sin(angle);
        }

        public Point(Point p, Vector v)
        {
            X = p.X + v.X;
            Y = p.Y + v.Y;
        }

        public double AngleTo(Point p)
        {
            return Math.Atan2(p.Y - Y, p.X - X);
        }

        public double DistanceTo(Point p)
        {
            return Math.Sqrt(MathUtil.Square(X - p.X) + MathUtil.Square(Y - p.Y));
        }

        public Point Translate(double dx, double dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point Rotate(double angle, Point origin = null)
        {
            origin = origin ?? Zero;

            return new Point(
                Math.Cos(angle) * (X - origin.X) - Math.Sin(angle) * (Y - origin.Y) + origin.X,
                Math.Sin(angle) * (X - origin.X) + Math.Cos(angle) * (Y - origin.Y) + origin.Y
            );
        }

        public Point Scale(double factor, Point origin = null)
        {
            if (origin == null)
                return new Point(X * factor, Y * factor);
            else
                return new Point((X - origin.X) * factor + origin.X, (Y - origin.Y) * factor + origin.Y);
        }

        public Point Transform(AffineMatrix2D matrix)
        {
            return new Point(
                X * matrix.xx + Y * matrix.xy + matrix.tx,
                X * matrix.yx + Y * matrix.yy + matrix.ty
            );
        }

        public Point ClosestPoint(Point p)
        {
            return this;
        }

        public Point Transform(Func<Point, Point> conversion)
        {
            return conversion(this);
        }

        public Rectangle BoundingBox()
        {
            return new Rectangle(this, this);
        }

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        public Point Mirror(Point aroundPoint)
        {
            return new Point(aroundPoint.X * 2 - X, aroundPoint.Y * 2 - Y);
        }

        public static Point operator +(Point p, Vector v) => new Point(p.X + v.X, p.Y + v.Y);
        public static Point operator -(Point p, Vector v) => new Point(p.X - v.X, p.Y - v.Y);

        public static Vector operator-(Point p1, Point p2) => new Vector(p1.X-p2.X, p1.Y-p2.Y);

        public static implicit operator (double x,double y)(Point p) => (p.X, p.Y);
        public static implicit operator Point((double x,double y) p) => new Point(p.x,p.y);

        public override bool Equals(object obj)
        {
            if (obj is Point point)
                return point.X == X && point.Y == Y;

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int) ((BitConverter.DoubleToInt64Bits(X) ^ BitConverter.DoubleToInt64Bits(Y)) * 2654435761);
            }
        }

#if DEBUG
        public override string ToString()
        {
            return $"({X},{Y})";
        }
#endif
    }
}
