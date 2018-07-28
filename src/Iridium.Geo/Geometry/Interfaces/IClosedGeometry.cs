namespace Iridium.Geo
{
    public interface IClosedGeometry : IGeometry, IOverlappable<IGeometry>
    {
        double Area { get; }
        bool IsPointInside(Point p);
    }
}