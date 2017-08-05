using System.Collections.Generic;

namespace Iridium.Geo
{
    public class Polyline : Poly, ILinearGeometry, ITransformable<Polyline>
    {
        public Polyline(IEnumerable<Point> points) : base(points, false)
        {
        }

        protected override Poly CreatePoly(IEnumerable<Point> points) => new Polyline(points);

        public double Length => Segments.Length();

        public new Polyline Scale(double factor, Point origin = null) => (Polyline) base.Scale(factor, origin);
        public new Polyline Rotate(double angle, Point origin = null) => (Polyline) base.Rotate(angle, origin);
        public new Polyline Translate(double dx, double dy) => (Polyline) base.Translate(dx, dy);
        public new Polyline Transform(AffineMatrix2D matrix) => (Polyline) base.Transform(matrix);

        public double StartAngle => Points[0].AngleTo(Points[1]);
        public double EndAngle => Points[Points.Count - 2].AngleTo(Points[Points.Count - 1]);
        public Point StartPoint => Points[0];
        public Point EndPoint => Points[Points.Count - 1];

        ILinearGeometry IScalable<ILinearGeometry>.Scale(double factor, Point origin) => Scale(factor, origin);
        ILinearGeometry IRotatable<ILinearGeometry>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ILinearGeometry ITranslatable<ILinearGeometry>.Translate(double dx, double dy) => Translate(dx, dy);
        ILinearGeometry ITransformable<ILinearGeometry>.Transform(AffineMatrix2D matrix) => Transform(matrix);
    }
}