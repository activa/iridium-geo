using System;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Geo.Drawing
{
	public class VectorImage
    {
        public readonly DrawableSpline[] Shapes;

        public VectorImage(IEnumerable<DrawableSpline> shapes)
        {
            Shapes = shapes.ToArray();
        }

        public Rectangle BoundingBox()
        {
			return Shapes.Select(shape => shape.Spline).BoundingBox();
        }

		public VectorImage Rotate(double angle, Point origin = null)
		{
			return new VectorImage(
				from shape in Shapes
				select new DrawableSpline(shape.Spline.Rotate(angle,origin), shape.FillColor, shape.LineColor, shape.LineWidth, shape.Alpha, shape.FillGradient)
			);
		}

		public VectorImage Scale(double scale, Point origin = null)
        {
            return new VectorImage(
                from shape in Shapes
                select new DrawableSpline(shape.Spline.Scale(scale,origin), shape.FillColor, shape.LineColor, shape.LineWidth*scale, shape.Alpha, shape.FillGradient)
                );
        }

        public VectorImage Translate(double dx, double dy)
        {
            return new VectorImage(
                from shape in Shapes
                select new DrawableSpline(shape.Spline.Translate(dx,dy), shape.FillColor, shape.LineColor, shape.LineWidth, shape.Alpha, shape.FillGradient)
                );

        }
    }
}