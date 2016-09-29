namespace Iridium.Geo.Geography
{
    public abstract class GeoProjection
    {
        public abstract Point ToPoint(LatLon latlon);
        public abstract LatLon ToLatLon(Point point);

        public static GeoProjection WebMercator = new WebMercatorProjection();
        public static GeoProjection Identity = new IdentityProjection();
    }
}