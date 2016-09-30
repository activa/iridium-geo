using System;

namespace Iridium.Geo
{
    public class Rectangle : IGeometry, ITranslatable<Rectangle>, IScalable<Rectangle>
    {
        public Point P1 { get; }
        public Point P2 { get; }

        public double MinX => P1.X;
        public double MinY => P1.Y;
        public double MaxX => P2.X;
        public double MaxY => P2.Y;

        public double Width => P2.X - P1.X;
        public double Height => P2.Y - P1.Y;

        public Rectangle(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Rectangle(Point p1, double w, double h)
        {
            P1 = p1;
            P2 = new Point(P1.X + w, P1.Y + h);
        }

        public bool Intersects(Rectangle r)
        {
            return !(r.MinX > MaxX || r.MaxX < MinX || r.MinY > MaxY || r.MaxY < MinY);
        }

        public Rectangle BoundingBox()
        {
            return this;
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx,dy);
        }

        public IGeometry Rotate(double angle, Point origin = null)
        {
            throw new NotSupportedException();
        }

        IGeometry IGeometry.Scale(double factor, Point origin)
        {
            return Scale(factor, origin);
        }

        public IGeometry Transform(AffineMatrix2D matrix)
        {
            throw new NotSupportedException();
        }

        public Point ClosestPoint(Point p)
        {
            return new Polygon(new[] {P1,new Point(P2.X,P1.Y),P2, new Point(P1.X,P2.Y)}).ClosestPoint(p);
        }

        public Point Center()
        {
            return new Point(P1.X + Width/2, P1.Y + Height/2);
        }

        public Rectangle Translate(double dx, double dy)
        {
            return new Rectangle(P1.Translate(dx,dy), P2.Translate(dx,dy));
        }

        public Rectangle Scale(double factor, Point origin)
        {
            return new Rectangle(P1.Scale(factor,origin), P2.Scale(factor,origin));
        }
    }

}
