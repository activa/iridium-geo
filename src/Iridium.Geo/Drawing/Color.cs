using System;

namespace Iridium.Geo.Drawing
{
    public class Color
    {
        public static Color Red => new Color(1,0,0);
        public static Color Green => new Color(0, 1, 0);
        public static Color Blue => new Color(0, 0, 1);

        public double A;

        private HSBValues _hsb;
        private RGBValues _rgb;

        public RGBValues RGB => _rgb ??= RGBValues.FromHSB(HSB);
        public HSBValues HSB => _hsb ??= HSBValues.FromRGB(RGB);

        public Color(Color c)
        {
            A = c.A;
            _rgb = new RGBValues(c.RGB.R,c.RGB.G,c.RGB.B);
        }

        public Color(double r, double g, double b, double a = 1.0)
        {
            _rgb = new RGBValues(r,g,b);

            A = a;
        }

        public Color(RGBValues rgb, double a = 1.0)
        {
            _rgb = rgb;

            A = a;
        }

        private Color(HSBValues hsb, double a = 1.0)
        {
            _hsb = hsb;

            A = a;
        }

        public Color Colorize(Color color)
        {
            return new Color(new HSBValues(color.HSB.H, HSB.S * color.HSB.S, HSB.B),A); 
        }

        public class RGBValues
        {
            public readonly double R, G, B;

            public RGBValues(double r, double g, double b)
            {
                R = r;
                G = g;
                B = b;
            }

            public static RGBValues FromHSB(HSBValues hsb)
            {
                if (hsb.S <= 0.001)
                    return new RGBValues(hsb.B, hsb.B, hsb.B);

                var temp1 = hsb.B < 0.5 ? hsb.B * (1 + hsb.S) : hsb.B + hsb.S - hsb.B * hsb.S;
                var temp2 = 2 * hsb.B - temp1;

                return new RGBValues(
                    ComponentValue(hsb.H / 360.0 + 1.0 / 3.0, temp1, temp2),
                    ComponentValue(hsb.H / 360.0, temp1, temp2),
                    ComponentValue(hsb.H / 360.0 - 1.0 / 3.0, temp1, temp2)
                    );
            }

            private static double ComponentValue(double value, double temp1, double temp2)
            {
                if (value < 0.0)
                    value += 1.0;
                else if (value > 1.0)
                    value -= 1.0;

                if (value * 6 < 1.0)
                    return temp2 + (temp1 - temp2) * 6 * value;
                if (value * 2 < 1.0)
                    return temp1;
                if (value * 3 < 2.0)
                    return temp2 + (temp1 - temp2) * (2.0 / 3.0 - value) * 6;

                return temp2;
            }


        }


        public class HSBValues
        {
            public readonly double H;
            public readonly double S;
            public readonly double B;

            public HSBValues(double h, double s, double b)
            {
                H = h;
                S = s;
                B = b;
            }

            public static HSBValues FromRGB(RGBValues rgb)
            {
                double min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
                double max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);

                if (max == min)
                {
                    return new HSBValues(0, 0, max);
                }

                double h, s, b;

                b = (max + min) / 2;

                if (b < 0.5)
                    s = (max - min) / (max + min);
                else
                    s = (max - min) / (2 - max - min);

                if (max == rgb.R)
                    h = (rgb.G - rgb.B) / (max - min);
                else if (max == rgb.G)
                    h = 2 + (rgb.B - rgb.R) / (max - min);
                else
                    h = 4 + (rgb.R - rgb.G) / (max - min);

                h *= 60;

                if (h < 0.0) h += 360;

                return new HSBValues(h, s, b);
            }
        }
    }
}