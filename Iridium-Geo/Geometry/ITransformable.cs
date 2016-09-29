namespace Iridium.Geo
{
	public interface ITranslatable<out T>
	{
		T Translate(double dx, double dy);
	}

    public interface IRotatable<out T>
    {
        T Rotate(double angle, Point origin = null);
    }

    public interface IScalable<out T>
    {
        T Scale(double factor, Point origin = null);
    }

    public interface ITransformable<out T> : IScalable<T>, IRotatable<T>, ITranslatable<T>
    {
        T Transform(AffineMatrix2D matrix);
    }
}
