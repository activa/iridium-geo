namespace Iridium.Geo
{
    public interface IOverlappable<in T> where T : IGeometry
    {
        bool Overlaps(T other);
    }
}