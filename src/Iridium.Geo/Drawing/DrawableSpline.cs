using System;

namespace Iridium.Geo.Drawing
{
    public class DrawableSpline
    {
        public readonly Spline Spline;
        public readonly Color FillColor;
        public readonly Color LineColor;
        public readonly double LineWidth;
        public readonly double Alpha;

        public DrawableSpline(Spline spline, Color fillColor, Color lineColor, double lineWidth, double alpha = 1.0, Gradient fillGradient = null)
        {
            Spline = spline;
            FillColor = fillColor;
            LineColor = lineColor;
            LineWidth = lineWidth;
            Alpha = alpha;
            FillGradient = fillGradient;
        }

        public Gradient FillGradient;

        public DrawableSpline Colorize(Color color, bool includeStrokes = false)
        {
            return new DrawableSpline(
                Spline,
                FillColor?.Colorize(color),
                includeStrokes ? LineColor?.Colorize(color) : LineColor,
                LineWidth,
                Alpha,
                FillGradient?.Colorize(color)
            );
        }
    }
}