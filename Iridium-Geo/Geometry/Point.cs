using System;

namespace Iridium.Geo
{
	public sealed class Point : IGeometry, ITransformable<Point>
    {
        public static Point Zero = new Point(0, 0);

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

        public double AngleTo(Point p)
        {
            return Math.Atan2(p.Y - Y, p.X - X);
        }

        public double DistanceTo(Point p)
        {
            return Math.Sqrt(MathUtil.Square(X - p.X) + MathUtil.Square(Y - p.Y));
        }

        public Point Translate(double dx,double dy)
        {
            return new Point(X+dx,Y+dy);
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
                return new Point((X-origin.X) * factor + origin.X, (Y-origin.Y) * factor + origin.Y);
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
	        return new Rectangle(this,this);
	    }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx,dy);
        }

        IGeometry IGeometry.Rotate(double angle, Point origin)
        {
            return Rotate(angle, origin);
        }

        IGeometry IGeometry.Scale(double factor, Point origin)
        {
            return Scale(factor, origin);
        }

        IGeometry IGeometry.Transform(AffineMatrix2D matrix)
        {
            return Transform(matrix);
        }

        public Point Mirror(Point aroundPoint)
        {
            return new Point(aroundPoint.X * 2 - X, aroundPoint.Y * 2 - Y);
        }

        public override bool Equals(object obj)
	    {
	        var p2 = obj as Point;

	        if (p2 != null)
	            return p2.X == X && p2.Y == Y;

	        return false;
	    }

	    public override int GetHashCode()
	    {
	        unchecked { return (int) ((BitConverter.DoubleToInt64Bits(X) ^ BitConverter.DoubleToInt64Bits(Y))*2654435761); }
        }

#if DEBUG
        public override string ToString()
        {
            return $"[Point: X={X}, Y={Y}]";
        }

#endif


    }


}
