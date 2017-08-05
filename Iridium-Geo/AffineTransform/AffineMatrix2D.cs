using System;

namespace Iridium.Geo
{
    public class AffineMatrix2D
    {
        // ReSharper disable InconsistentNaming
        public readonly double xx, xy, yx, yy, tx, ty;
        // ReSharper restore InconsistentNaming

        public static class Factory
        {
            public static AffineMatrix2D Scale(double x, double y) => new AffineMatrix2D(xx: x, yy: y);
            public static AffineMatrix2D Translate(double x, double y) => new AffineMatrix2D(xx: 1, yy: 1, tx: x, ty: y);
            public static AffineMatrix2D Rotate(double angle) => new AffineMatrix2D(xx: Math.Cos(angle), xy: -Math.Sin(angle), yx: Math.Sin(angle), yy: Math.Cos(angle));
            public static AffineMatrix2D Rotate(double angle, Point p) => Translate(-p.X, -p.Y).Rotate(angle).Translate(p.X, p.Y);
            public static AffineMatrix2D Mirror(Point p) => new AffineMatrix2D(xx:-1,yy:-1,tx:p.X*2,ty:p.Y*2);
            public static AffineMatrix2D MirrorX = Scale(1,-1);
            public static AffineMatrix2D MirrorY = Scale(-1, 1);
            public static AffineMatrix2D Mirror(LineSegment seg) => Translate(-seg.P1.X,-seg.P1.Y).Rotate(-seg.Angle).Scale(1,-1).Rotate(seg.Angle).Translate(seg.P1.X,seg.P1.Y);
        }

        private AffineMatrix2D()
        {
            this.xx = 1;
            this.yy = 1;
        }

        public AffineMatrix2D(double xx=1, double xy=0, double yx=0, double yy=1, double tx=0, double ty=0)
        {
            this.xx = xx;
            this.xy = xy;
            this.yx = yx;
            this.yy = yy;
            this.tx = tx;
            this.ty = ty;
        }

        public static AffineMatrix2D Identity = new AffineMatrix2D();

        public AffineMatrix2D Scale(double x, double y) => Factory.Scale(x, y) * this;
        public AffineMatrix2D Translate(double x, double y) => Factory.Translate(x, y) * this;
        public AffineMatrix2D Rotate(double angle) => Factory.Rotate(angle) * this;
        public AffineMatrix2D Rotate(double angle, Point p) => Factory.Rotate(angle, p) * this;
        public AffineMatrix2D Mirror(Point p) => Factory.Mirror(p) * this;
        public AffineMatrix2D MirrorX => Factory.MirrorX * this;
        public AffineMatrix2D MirrorY => Factory.MirrorY * this;
        public AffineMatrix2D Mirror(LineSegment seg) => Factory.Mirror(seg) * this;
        public AffineMatrix2D Transform(AffineMatrix2D matrix) => matrix * this;

        public static AffineMatrix2D operator *(AffineMatrix2D m1, AffineMatrix2D m2)
        {
            return new AffineMatrix2D
            (
                xx : m1.xx * m2.xx + m1.xy * m2.yx,
                xy : m1.xx * m2.xy + m1.xy * m2.yy, 
                yx : m1.yx * m2.xx + m1.yy * m2.yx,
                yy : m1.yx * m2.xy + m1.yy * m2.yy,
                tx : m1.xx * m2.tx + m1.xy * m2.ty + m1.tx,
                ty : m1.yx * m2.tx + m1.yy * m2.ty + m1.ty
            );
        }

    }
}