namespace Iridium.Geo
{
    public interface ILinearGeometry : IGeometry, ITransformable<ILinearGeometry>, IIntersectable<ILinearGeometry>
    {
        double Length { get; }

        double StartAngle { get; }
        double EndAngle { get; }

        Point StartPoint { get; }
        Point EndPoint { get; }
    }
}