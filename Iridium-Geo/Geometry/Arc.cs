using System;

namespace Iridium.Geo
{
    public class Arc : ILinearGeometry, ITranslatable<Arc>, IRotatable<Arc>, IScalable<Arc>
    {
        public Circle Circle { get; }
        public double FromAngle { get; }
        public double ToAngle { get; }
        public bool Increasing { get; }

        public Arc(Circle circle, double startAngle, double endAngle, bool clockWise = true)
        {
            Circle = circle;
            FromAngle = startAngle;
            ToAngle = endAngle;
            Increasing = clockWise;
        }

        public double Length => GeometryUtil.ArcLength(Circle.Radius, Increasing ? GeometryUtil.NormalizeAngle(ToAngle - FromAngle) : GeometryUtil.NormalizeAngle(FromAngle - ToAngle));

        public double EndPointDirectionAngle => Increasing ? GeometryUtil.NormalizeAngle(ToAngle + Math.PI / 2) : GeometryUtil.NormalizeAngle(ToAngle - Math.PI/2);

        public Rectangle BoundingBox()
        {
            throw new NotImplementedException();
        }

        IGeometry IGeometry.Translate(double dx, double dy) => Translate(dx, dy);
        IGeometry IGeometry.Rotate(double angle, Point origin) => Rotate(angle, origin);
        IGeometry IGeometry.Scale(double factor, Point origin) => Scale(factor, origin);

        IGeometry IGeometry.Transform(AffineMatrix2D matrix)
        {
            throw new NotImplementedException();
        }

        public Arc Translate(double dx, double dy)
        {
            return new Arc(Circle.Translate(dx, dy), FromAngle, ToAngle, Increasing);
        }

        public Arc Rotate(double angle, Point origin)
        {
            return new Arc(
                new Circle(Circle.Center.Rotate(angle, origin), Circle.Radius),
                FromAngle + angle,
                ToAngle + angle,
                Increasing);
        }

        public Point ClosestPoint(Point p)
        {
            throw new NotImplementedException();
        }

        public Point StartPoint => new Point(Circle.Center, Increasing ? FromAngle : ToAngle, Circle.Radius);
        public Point EndPoint => new Point(Circle.Center, Increasing ? ToAngle : FromAngle, Circle.Radius);
        public double StartAngle => GeometryUtil.NormalizeAngle(Increasing ? (FromAngle + Math.PI / 2) : (FromAngle - Math.PI / 2));
        public double EndAngle => GeometryUtil.NormalizeAngle(Increasing ? (ToAngle + Math.PI / 2) : (ToAngle - Math.PI / 2));

        ILinearGeometry IScalable<ILinearGeometry>.Scale(double factor, Point origin) => Scale(factor, origin);
        ILinearGeometry IRotatable<ILinearGeometry>.Rotate(double angle, Point origin) => Rotate(angle, origin);
        ILinearGeometry ITranslatable<ILinearGeometry>.Translate(double dx, double dy) => Translate(dx, dy);

        ILinearGeometry ITransformable<ILinearGeometry>.Transform(AffineMatrix2D matrix)
        {
            throw new NotImplementedException();
        }

        public Arc Scale(double factor, Point origin)
        {
            return new Arc(
                new Circle(Circle.Center.Scale(factor, origin), Circle.Radius * factor),
                            FromAngle,
                            ToAngle,
                            Increasing
                      );
        }

    }

}
