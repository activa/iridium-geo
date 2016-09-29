using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo
{
    public class Spline : IGeometry, ITransformable<Spline>
    {
        public IReadOnlyList<BezierCurve> Curves { get; }
        public bool Closed { get; }

        private Rectangle _boundingBox;

        public Spline(IEnumerable<BezierCurve> curves, bool closed)
        {
            Curves = curves.ToArray();
            Closed = closed;
        }

        public Point ClosestPoint(Point p)
        {
            Point closest = null;
            double minDistance = double.MaxValue;

            foreach (var curve in Curves)
            {
                double distance;

                Point cp = curve.ClosestPoint(p, out distance);

                if (distance < minDistance)
                {
                    closest = cp;
                    minDistance = distance;
                }
            }

            return closest;


        }

        public Spline Scale(double factor, Point origin = null)
        {
            return new Spline(Curves.Scale(factor, origin), Closed);
        }

        public Spline Translate(double dx, double dy)
        {
            return new Spline(Curves.Translate(dx,dy), Closed);
        }

		public Spline Rotate(double angle, Point origin = null)
		{
			return new Spline(Curves.Rotate(angle,origin), Closed);
		}

        public Spline Transform(AffineMatrix2D matrix)
        {
            return new Spline(Curves.Transform(matrix), Closed);
        }


		public Rectangle BoundingBox()
        {
            if (_boundingBox != null)
                return _boundingBox;

            double x1=double.MaxValue, y1=double.MaxValue, x2=double.MinValue, y2=double.MinValue;

            foreach (var curve in Curves)
            {
                var box = curve.BoundingBox();

                x1 = Math.Min(x1, box.MinX);
                y1 = Math.Min(y1, box.MinY);
                x2 = Math.Max(x2, box.MaxX);
                y2 = Math.Max(y2, box.MaxY);
            }

            return (_boundingBox = new Rectangle(new Point(x1,y1), new Point(x2,y2) ));
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
    }

}
