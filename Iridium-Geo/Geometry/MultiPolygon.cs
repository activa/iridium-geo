using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiPolyline : IMultiGeometry<Polyline>, ITransformable<MultiPolyline>, IIntersectable<Poly>, IIntersectable<Polyline>, IIntersectable<Polygon>
    {
        public IReadOnlyList<Polyline> Polylines { get; }

        public MultiPolyline(IEnumerable<Polyline> polylines)
        {
            Polylines = polylines.ToArray();
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

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }

        public Point ClosestPoint(Point p)
        {
            return Polylines.ClosestPoint(p);
        }

        public Rectangle BoundingBox()
        {
            return Polylines.BoundingBox();
        }

        public MultiPolyline Scale(double factor, Point origin = null)
        {
            return new MultiPolyline(Polylines.Scale<Polyline>(factor, origin));
        }

        public MultiPolyline Rotate(double angle, Point origin = null)
        {
            return new MultiPolyline(Polylines.Rotate<Polyline>(angle, origin));
        }

        public MultiPolyline Translate(double dx, double dy)
        {
            return new MultiPolyline(Polylines.Translate<Polyline>(dx,dy));
        }

        public MultiPolyline Transform(AffineMatrix2D matrix)
        {
            return new MultiPolyline(Polylines.Transform<Polyline>(matrix));
        }

        IEnumerator<Polyline> IEnumerable<Polyline>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Polyline> GetEnumerator()
        {
            return Polylines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Intersects(Poly p)
        {
            return Polylines.Any(p.Intersects);
        }

        public IEnumerable<Point> Intersections(Poly other)
        {
            throw new System.NotImplementedException();
        }

        public bool Intersects(Polyline p)
        {
            return Polylines.Any(p.Intersects);
        }

        public IEnumerable<Point> Intersections(Polyline other)
        {
            throw new System.NotImplementedException();
        }

        public bool Intersects(Polygon p)
        {
            return Polylines.Any(p.Intersects);
        }

        public IEnumerable<Point> Intersections(Polygon other)
        {
            throw new System.NotImplementedException();
        }
    }
}