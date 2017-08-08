using System;
using System.Collections;
using System.Collections.Generic;

namespace Iridium.Geo
{
    public class Rectangle : IClosedGeometry, ITranslatable<Rectangle>, IScalable<Rectangle>, ITransformable<Polygon>
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

        public static implicit operator Polygon(Rectangle rectangle)
        {
            return new Polygon(rectangle.CornerPoints());
        }

        public IEnumerable<Point> CornerPoints()
        {
            yield return P1;
            yield return new Point(MaxX,MinY);
            yield return P2;
            yield return new Point(MinX,MaxY);
        }

        public Rectangle BoundingBox()
        {
            return this;
        }

        public Point ClosestPoint(Point p) => new Polygon(CornerPoints()).ClosestPoint(p);

        public Point Center => new Point(P1.X + Width/2, P1.Y + Height/2);

        public Rectangle Translate(double dx, double dy) => new Rectangle(P1.Translate(dx,dy), P2.Translate(dx,dy));
        public Rectangle Scale(double factor, Point origin) => new Rectangle(P1.Scale(factor,origin), P2.Scale(factor,origin));
        public Polygon Rotate(double angle, Point origin = null) => new Polygon(CornerPoints().Rotate(angle,origin));
        public Polygon Transform(AffineMatrix2D matrix) => new Polygon(CornerPoints().Transform(matrix));

        public double Area => Width * Height;

        public bool IsPointInside(Point p) => p.X >= P1.X && p.X <= P2.X && p.Y >= P1.Y && p.Y <= P2.Y;

        public bool Overlaps(IGeometry geom)
        {
            switch (geom)
            {
                case Point p:
                    return IsPointInside(p);
                default:
                    throw new NotImplementedException();
            }
        }

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        Polygon IScalable<Polygon>.Scale(double factor, Point origin) => new Polygon(CornerPoints()).Scale(factor,origin);
        Polygon ITranslatable<Polygon>.Translate(double dx, double dy) => new Polygon(CornerPoints().Translate(dx,dy));
    }

}
