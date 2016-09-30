using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiGeometry : IMultiGeometry, ITransformable<MultiGeometry>
    {
        public IReadOnlyList<IGeometry> Geometries { get; }

        public MultiGeometry(IEnumerable<IGeometry> geometries)
        {
            Geometries = geometries.ToArray();
        }

        public IEnumerator<IGeometry> GetEnumerator()
        {
            return Geometries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Rectangle BoundingBox()
        {
            return Geometries.BoundingBox();
        }

        IGeometry IGeometry.Translate(double dx, double dy)
        {
            return Translate(dx,dy);
        }

        IGeometry IGeometry.Rotate(double angle, Point origin)
        {
            return Rotate(angle,origin);
        }

        IGeometry IGeometry.Scale(double factor, Point origin)
        {
            return Scale(factor, origin);
        }

        IGeometry IGeometry.Transform(AffineMatrix2D matrix)
        {
            return Transform(matrix);
        }

        public MultiGeometry Scale(double factor, Point origin = null)
        {
            return new MultiGeometry(Geometries.Select(g => g.Scale(factor, origin)));
        }

        public MultiGeometry Rotate(double angle, Point origin = null)
        {
            return new MultiGeometry(Geometries.Select(g => g.Rotate(angle, origin)));
        }

        public MultiGeometry Translate(double dx, double dy)
        {
            return new MultiGeometry(Geometries.Select(g => g.Translate(dx, dy)));
        }

        public MultiGeometry Transform(AffineMatrix2D matrix)
        {
            return new MultiGeometry(Geometries.Select(g => g.Transform(matrix)));
        }
    }
}