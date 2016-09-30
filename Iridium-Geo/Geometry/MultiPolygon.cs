using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiPolygon : IMultiGeometry, ITransformable<MultiPolygon>, IEnumerable<Polygon>
    {
        public IReadOnlyList<Polygon> Polygons { get; }

        public MultiPolygon(IEnumerable<Polygon> polygons)
        {
            Polygons = polygons.ToArray();
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

        public Point ClosestPoint(Point p)
        {
            return Polygons.ClosestPoint(p);
        }

        public MultiPolygon Scale(double factor, Point origin = null)
        {
            return new MultiPolygon(Polygons.Scale(factor, origin));
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx, dy);
        }

        public MultiPolygon Rotate(double angle, Point origin = null)
        {
            return new MultiPolygon(Polygons.Rotate(angle, origin));
        }

        public Rectangle BoundingBox()
        {
            return Polygons.BoundingBox();
        }

        public MultiPolygon Translate(double dx, double dy)
        {
            return new MultiPolygon(Polygons.Translate(dx,dy));
        }

        public MultiPolygon Transform(AffineMatrix2D matrix)
        {
            return new MultiPolygon(Polygons.Transform(matrix));
        }

        IEnumerator<IGeometry> IEnumerable<IGeometry>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Polygon> GetEnumerator()
        {
            return Polygons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}