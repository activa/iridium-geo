using System;
using System.Collections;

namespace Iridium.Geo
{
    public interface IGeometry
    {
        Rectangle BoundingBox();

        IGeometry Translate(double dx, double dy);
        IGeometry Rotate(double angle, Point origin = null);
        IGeometry Scale(double factor, Point origin = null);
        IGeometry Transform(AffineMatrix2D matrix);

        Point ClosestPoint(Point p);
    }
}