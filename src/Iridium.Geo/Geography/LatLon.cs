#region License
//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2016 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using Iridium.Units;

namespace Iridium.Geo.Geography
{
    public class LatLon
    {
        private readonly Point _point;

        public double Lat => _point.Y;
        public double Lon => _point.X;

        public double LatRadians => Lat*MathUtil.DegtoRadFactor;
        public double LonRadians => Lon*MathUtil.DegtoRadFactor;

        public LatLon(double lat, double lon)
        {
            _point = new Point(lon,lat);
        }

        public LatLon(Point p)
        {
            _point = p;
        }

        public LatLon(Point p, GeoProjection projection)
        {
            var latlon = projection.ToLatLon(p);

            _point = new Point(latlon.Lon, latlon.Lat);
        }

        public LatLon(string hash)
        {
            var latlon = Geohash.Decode(hash);

            _point = new Point(latlon.Lon, latlon.Lat);
        }

        public string ToGeohash(int precision) => Geohash.Encode(this, precision);

        public static implicit operator Point(LatLon latlon)
        {
            return latlon._point;
        }

        public static implicit operator LatLon(Point point)
        {
            return new LatLon(point);
        }

        // ===Distance functions

        public NumberWithUnit DistanceTo(LatLon p2)
        {
            return Distance(Lat, Lon, p2.Lat, p2.Lon);
        }

        public double DistanceTo(LatLon p2, Unit unit)
        {
            return Distance(Lat, Lon, p2.Lat, p2.Lon, unit);
        }

        public double DistanceMetersTo(LatLon p2)
        {
            return DistanceMeters(Lat, Lon, p2.Lat, p2.Lon);
        }

        public Point ToPoint(GeoProjection projection)
        {
            return projection.ToPoint(this);
        }

        public static double DistanceMeters(string hash1, string hash2)
        {
            return Geohash.Decode(hash1).DistanceMetersTo(Geohash.Decode(hash2));
        }

        public static double Distance(string hash1, string hash2, Unit unit)
        {
            return Geohash.Decode(hash1).DistanceTo(Geohash.Decode(hash2), unit);
        }

        public static NumberWithUnit Distance(string hash1, string hash2)
        {
            return Geohash.Decode(hash1).DistanceTo(Geohash.Decode(hash2));
        }

        public static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371008.8;
            const double f = Math.PI / 180;

            lat1 *= f;
            lat2 *= f;
            lon1 *= f;
            lon2 *= f;

            double angle = Haversine(lat2 - lat1) + Math.Cos(lat1) * Math.Cos(lat2) * Haversine(lon2 - lon1);

            return 2 * earthRadius * Math.Asin(Math.Min(1, Math.Sqrt(angle)));
        }

        public static double Distance(double lat1, double lon1, double lat2, double lon2, Unit unit)
        {
            double meters = DistanceMeters(lat1, lon1, lat2, lon2);

            return meters.ConvertFrom(Unit.Meters).To(unit);
        }

        public static NumberWithUnit Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double meters = DistanceMeters(lat1, lon1, lat2, lon2);

            return meters.In(Unit.Meters);
        }

        private static double Haversine(double angle)
        {
            return (1 - Math.Cos(angle)) / 2;
        }

    }
}