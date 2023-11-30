using System;
using System.Xml.Linq;

namespace Iridium.Geo
{
    public class Vector
    {
        public readonly double X;
        public readonly double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static double operator *(Vector v1, Vector v2) => v1.X * v2.X + v1.Y * v2.Y;
        public static Vector operator *(Vector v, double r) => new Vector(v.X * r, v.Y * r);
        public static Vector operator *(double r, Vector v) => new Vector(v.X * r, v.Y * r);
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X+v2.X, v1.Y+v2.Y);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.X-v2.X, v1.Y-v2.Y);
        public static Vector operator -(Vector v) => new Vector(-v.X, -v.Y);
        public static implicit operator (double x,double y)(Vector v) => (v.X, v.Y);

        public double Magnitude => Math.Sqrt(X * X + Y * Y);
        public double Angle => Math.Atan2(Y, X);

        public Vector Normalized()
        {
            return new Vector(X / Magnitude, Y / Magnitude);
        }

    }
}