using System;

namespace Iridium.Geo
{
    public class Ellipse : IGeometry, ITransformable<Ellipse>
    {
        public LineSegment CordSegment { get; }
        public double Cord { get; }
        public Point Center { get; }
        public double RadiusX { get; }
        public double RadiusY { get; }
        public double Angle { get; }

        public Ellipse(Point center, double radiusX, double radiusY, double angle = 0.0)
        {
            double maxRadius = Math.Max(radiusX, radiusY);
            double minRadius = Math.Min(radiusX, radiusY);

            Cord = 2*maxRadius;

            double a = Cord*Cord/4 - minRadius*minRadius;

            if (maxRadius > minRadius)
            {
                CordSegment = new LineSegment(new Point(center.X - Math.Sqrt(a), center.Y),new Point(center.Y + Math.Sqrt(a), center.Y)).Rotate(angle,center);
            }
            else
            {
                CordSegment = new LineSegment(new Point(center.X, center.Y - Math.Sqrt(a)),new Point(center.X, center.Y + Math.Sqrt(a))).Rotate(angle,center);
            }

            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            Angle = angle;

        }

        public Ellipse(Point fp1, Point fp2, double cord)
        {
            CordSegment = new LineSegment(fp1,fp2);
            Cord = cord;

            Center = CordSegment.Center;
            Angle = CordSegment.Angle;
            RadiusX = Cord/2;
            RadiusY = Math.Sqrt(cord*cord/4 - CordSegment.Length*CordSegment.Length/4);
        }

        public Ellipse(Circle circle) : this(circle.Center, circle.Radius, circle.Radius)
        {
        }

        public Point PointAt(double t)
        {
            return new Point(
                        Center.X + RadiusX * Math.Cos(t) * Math.Cos(Angle) - RadiusY * Math.Sin(t) * Math.Sin(Angle),
                        Center.Y + RadiusY * Math.Sin(t) * Math.Cos(Angle) + RadiusY * Math.Cos(t) * Math.Sin(Angle)
                );
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

        public Ellipse Scale(double factor, Point origin = null)
        {
            return new Ellipse(Center.Scale(factor,origin), RadiusX*factor, RadiusY * factor, Angle);
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }

        public Ellipse Rotate(double angle, Point origin = null)
        {
            return new Ellipse(Center.Rotate(angle,origin), RadiusX, RadiusY, Angle + angle);
        }

        public Rectangle BoundingBox()
        {
            double t;

            t = Math.Atan(-RadiusY*Math.Tan(Angle)/RadiusX);

            var x1 = Center.X + RadiusX*Math.Cos(t)*Math.Cos(Angle) - RadiusY*Math.Sin(t)*Math.Sin(Angle);
            var x2 = Center.X + RadiusX*Math.Cos(t + Math.PI)*Math.Cos(Angle) - RadiusY*Math.Sin(t + Math.PI)*Math.Sin(Angle);

            t = Math.Atan(RadiusY*(1/Math.Tan(Angle))/RadiusX);

            var y1 = Center.Y + RadiusY*Math.Sin(t)*Math.Cos(Angle) + RadiusX*Math.Cos(t)*Math.Sin(Angle);
            var y2 = Center.Y + RadiusY*Math.Sin(t + Math.PI)*Math.Cos(Angle) + RadiusX*Math.Cos(t + Math.PI)*Math.Sin(Angle);


            return new Rectangle(new Point(Math.Min(x1, x2), Math.Min(y1, y2)), new Point(Math.Max(x1, x2), Math.Max(y1, y2)));
        }

        public Ellipse Translate(double dx, double dy)
        {
            return new Ellipse(Center.Translate(dx,dy), RadiusX, RadiusY, Angle);
        }

        public Ellipse Transform(AffineMatrix2D matrix)
        {
            throw new NotImplementedException();
        }
    }
}