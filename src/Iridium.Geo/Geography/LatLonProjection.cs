namespace Iridium.Geo.Geography
{
    public class LatLonProjection : GeoProjection
    {
        public override Point ToPoint(LatLon latlon)
        {
            return new Point(latlon.Lon, latlon.Lat);
        }

        public override LatLon ToLatLon(Point point)
        {
            return new LatLon(point.Y, point.X);
        }
    }
}