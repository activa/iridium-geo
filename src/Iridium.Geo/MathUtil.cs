using System;

namespace Iridium.Geo
{
    public static class MathUtil
    {
        public const double RadToDegFactor = 180.0/Math.PI;
        public const double DegtoRadFactor = Math.PI/180.0;

        public static double Square(double n) => n*n;
        public static double Cube(double n) => n*n*n;

        public static double Min(double v1,double v2,double v3, double v4) => Math.Min(v1, Math.Min(v2, Math.Min(v3, v4)));
        public static double Max(double v1,double v2,double v3, double v4) => Math.Max(v1, Math.Max(v2, Math.Max(v3, v4)));
        public static double Min(double v1, double v2, double v3) => Math.Min(v1, Math.Min(v2, v3));
        public static double Max(double v1, double v2, double v3) => Math.Max(v1, Math.Max(v2, v3));

        public static double Cotangent(double a) => 1/Math.Tan(a);

        public static double RadToDeg(double rad) => rad * RadToDegFactor;
        public static double DegToRad(double deg) => deg * DegtoRadFactor;
    }
}