using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiPolygon : IGeometry, ITransformable<MultiPolygon>
    {
        public IReadOnlyList<Polygon> Polygons { get; }

        public MultiPolygon(IEnumerable<Polygon> polygons)
        {
            Polygons = polygons.ToArray();
        }

        public Rectangle BoundingBox()
        {
            return Polygons.BoundingBox();
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx,dy);
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

        public MultiPolygon Scale(double factor, Point origin = null)
        {
            return new MultiPolygon(Polygons.Scale(factor, origin));
        }

        public MultiPolygon Rotate(double angle, Point origin = null)
        {
            return new MultiPolygon(Polygons.Rotate(angle, origin));
        }

        public MultiPolygon Translate(double dx, double dy)
        {
            return new MultiPolygon(Polygons.Translate(dx,dy));
        }

        public MultiPolygon Transform(AffineMatrix2D matrix)
        {
            return new MultiPolygon(Polygons.Transform(matrix));
        }
    }
}