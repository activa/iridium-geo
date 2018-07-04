using System;

namespace Iridium.Geo.Geography
{
    public class WebMercatorProjection : GeoProjection
    {
        public override Point ToPoint(LatLon latlon)
        {
            double x = (latlon.Lon + 180) / 360;
            double y = (1 - Math.Log(Math.Tan(latlon.LatRadians) + 1 / Math.Cos(latlon.LatRadians)) / Math.PI) / 2;

            return new Point(x,y);
        }

        public override LatLon ToLatLon(Point point)
        {
            return new LatLon(MathUtil.RadToDeg(Math.Atan(Math.Sinh(Math.PI - (2.0 * Math.PI * point.Y)))), point.X * 360.0 - 180);
       }
    }
}