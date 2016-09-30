using System;

namespace Iridium.Geo
{
    public class Circle : IGeometry, ITranslatable<Circle>, IRotatable<Circle>, IScalable<Circle>, ITransformable<Ellipse>
    {
        public Point Center { get; }
        public double Radius { get; }

        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(Point a, Point b, Point c)
        {
            double aSlope = (b.Y - a.Y) / (b.X - a.X);
            double bSlope = (c.Y - b.Y) / (c.X - b.X);

            double cX = (aSlope*bSlope*(a.Y - c.Y) + bSlope*(a.X + b.X) - aSlope*(b.X+c.X) )/(2* (bSlope-aSlope) );
            double cY = -1*(cX - (a.X+b.X)/2)/aSlope +  (a.Y+b.Y)/2;

            Center = new Point(cX, cY);
            Radius = a.DistanceTo(Center);
        }

        public Circle Translate(double dx, double dy)
        {
            return new Circle(Center.Translate(dx, dy), Radius);
        }

	    public Circle Rotate(double angle, Point origin = null)
	    {
            return new Circle(Center.Rotate(angle, origin), Radius);
	    }

		public Circle Scale(double factor, Point origin = null)
		{
			return new Circle(Center.Scale(factor, origin), Radius * factor);
		}

	    public Ellipse Transform(AffineMatrix2D matrix)
	    {
	        throw new NotImplementedException();
	    }

        public Point ClosestPoint(Point p)
        {
            return new Point(p, Center.AngleTo(p), Radius);
        }

        IGeometry IGeometry.Transform(AffineMatrix2D matrix)
        {
            return Transform(matrix);
        }

        public Rectangle BoundingBox()
        {
            return new Rectangle(new Point(Center.X - Radius, Center.Y - Radius), Radius * 2, Radius * 2);
        }

        IGeometry IGeometry.Rotate(double angle, Point origin)
        {
            return Rotate(angle, origin);
        }

        IGeometry IGeometry.Scale(double factor, Point origin)
        {
            return Scale(factor, origin);
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }

        Ellipse IScalable<Ellipse>.Scale(double factor, Point origin)
        {
            return new Ellipse(Scale(factor,origin));
        }

        Ellipse IRotatable<Ellipse>.Rotate(double angle, Point origin)
        {
            return new Ellipse(Rotate(angle,origin));
        }

        Ellipse ITranslatable<Ellipse>.Translate(double dx, double dy)
        {
            return new Ellipse(Translate(dx,dy));
        }
    }
}
