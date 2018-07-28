using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class MultiGeometry : IMultiGeometry<IGeometry>, ITransformable<MultiGeometry>
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

        public Rectangle BoundingBox()
        {
            return Geometries.BoundingBox();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx,dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle,origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);
        IGeometry IGeometry.Transform(AffineMatrix2D matrix) => Transform(matrix);

        public Point ClosestPoint(Point p)
        {
            return Geometries.ClosestPoint(p);
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