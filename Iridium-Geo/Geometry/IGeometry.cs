using System;
using System.Collections;
using System.Collections.Generic;

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

    public interface IIntersectable<in T> where T : IGeometry
    {
        bool Intersects(T other);
    }

    public interface ILinearGeometry : IGeometry, ITransformable<ILinearGeometry>
    {
        double Length { get; }

        double StartAngle { get; }
        double EndAngle { get; }

        Point StartPoint { get; }
        Point EndPoint { get; }
    }

    public interface ICurve : ILinearGeometry, ITransformable<ICurve>
    {
        Point PointOnCurve(double t);
        IEnumerable<Point> GeneratePoints(int n);
        IEnumerable<LineSegment> Partition(int numSegments);
    }

    public interface IClosedGeometry
    {
        double Area { get; }
        bool IsPointInside(Point p);
    }

}