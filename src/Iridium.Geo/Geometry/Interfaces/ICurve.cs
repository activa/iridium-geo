using System.Collections.Generic;

namespace Iridium.Geo
{
    public interface ICurve : ILinearGeometry, ITransformable<ICurve>
    {
        Point PointOnCurve(double t);
        IEnumerable<Point> GeneratePoints(int n);
        IEnumerable<LineSegment> Partition(int numSegments);
    }
}