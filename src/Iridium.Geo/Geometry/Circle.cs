using System;
using System.Collections.Generic;

namespace Iridium.Geo
{
    public class Circle : IClosedGeometry, 
                          ITranslatable<Circle>, IRotatable<Circle>, IScalable<Circle>, ITransformable<Ellipse>, 
                          IIntersectable<Circle>, 
                          IOverlappable<Point>, IOverlappable<Circle>
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


        public Rectangle BoundingBox()
        {
            return new Rectangle(new Point(Center.X - Radius, Center.Y - Radius), Radius * 2, Radius * 2);
        }

        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        Ellipse IScalable<Ellipse>.Scale(double factor, Point origin) => new Ellipse(Scale(factor,origin));
        Ellipse IRotatable<Ellipse>.Rotate(double angle, Point origin) => new Ellipse(Rotate(angle,origin));
        Ellipse ITranslatable<Ellipse>.Translate(double dx, double dy) => new Ellipse(Translate(dx,dy));

        public bool Intersects(Circle other)
        {
            return Center.DistanceTo(other.Center) <= (Radius + other.Radius);
        }

        public IEnumerable<Point> Intersections(Circle other)
        {
            throw new NotImplementedException();
        }

        public double DistanceTo(Circle other)
        {
            return Math.Max(Center.DistanceTo(other.Center) - Radius - other.Radius, 0);
        }

        public double DistanceTo(Point other)
        {
            return Math.Max(Center.DistanceTo(other) - Radius, 0);
        }

        public double Area => Radius * Radius * Math.PI;

        public bool IsPointInside(Point p)
        {
            return Center.DistanceTo(p) <= Radius;
        }

        public bool Overlaps(IGeometry geom)
        {
            switch (geom)
            {
                case Point p:
                    return Overlaps(p);

                case Circle c:
                    return Overlaps(c);

                default:
                    throw new NotImplementedException();
            }
        }

        public bool Overlaps(Point other)
        {
            return IsPointInside(other);
        }

        public bool Overlaps(Circle other)
        {
            return Intersects(other);
        }
    }
}
