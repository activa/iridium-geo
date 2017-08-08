using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class Polygon : Poly, IClosedGeometry, ITransformable<Polygon>
    {
        public Polygon(IEnumerable<Point> points) : base(points, true)
        {
        }

        protected override Poly CreatePoly(IEnumerable<Point> points) => new Polygon(points);

        public new Polygon Scale(double factor, Point origin = null) => (Polygon)base.Scale(factor, origin);
        public new Polygon Rotate(double angle, Point origin = null) => (Polygon)base.Rotate(angle, origin);
        public new Polygon Translate(double dx, double dy) => (Polygon)base.Translate(dx, dy);
        public new Polygon Transform(AffineMatrix2D matrix) => (Polygon)base.Transform(matrix);

        public double Area
        {
            get
            {
                double area = 0;

                Point p1 = Points[Points.Count - 1];

                foreach (var p2 in Points)
                {
                    area += (p1.X + p2.X) * (p1.Y - p2.Y);

                    p1 = p2;
                }

                return Math.Abs(area/2);
            }
        }

        public bool Overlaps(IGeometry geom)
        {
            switch (geom)
            {
                case Poly p:
                    return base.Intersects(p);
                case Point p:
                    return base.IsPointInside(p);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}