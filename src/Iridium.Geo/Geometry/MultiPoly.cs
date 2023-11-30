using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiPoly : IMultiGeometry<Poly>, ITransformable<MultiPoly>, IIntersectable<Poly>
    {
        public IReadOnlyList<Poly> Polylines { get; }

        public MultiPoly(IEnumerable<Poly> polylines)
        {
            Polylines = polylines.ToArray();
        }

        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);
        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);

        public Point ClosestPoint(Point p) => Polylines.ClosestPoint(p);
        public Rectangle BoundingBox() => Polylines.BoundingBox();

        public MultiPoly Scale(double factor, Point origin = null) => new MultiPoly(Polylines.Scale(factor, origin));
        public MultiPoly Rotate(double angle, Point origin = null) => new MultiPoly(Polylines.Rotate(angle, origin));
        public MultiPoly Translate(double dx, double dy) => new MultiPoly(Polylines.Translate(dx, dy));
        public MultiPoly Transform(AffineMatrix2D matrix) => new MultiPoly(Polylines.Transform(matrix));

        IEnumerator<Poly> IEnumerable<Poly>.GetEnumerator() => GetEnumerator();
        private IEnumerator<Poly> GetEnumerator() => Polylines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Intersects(Poly p) => Polylines.Any(p.Intersects);

        public IEnumerable<Point> Intersections(Poly other) => throw new NotImplementedException();
    }
}