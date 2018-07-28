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

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Vector Normalized()
        {
            return new Vector(X / Length, Y / Length);
        }

        public static Vector operator *(Vector v, double r)
        {
            return new Vector(v.X * r, v.Y * r);
        }

        public static implicit operator (double x,double y)(Vector v) => (v.X, v.Y);
    }
}