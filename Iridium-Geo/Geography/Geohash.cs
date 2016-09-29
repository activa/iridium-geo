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
using System.Text;
using Iridium.Core;

namespace Iridium.Geo.Geography
{
    public static class Geohash
    {
        private enum Direction
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        private const string _BASE32 = "0123456789bcdefghjkmnpqrstuvwxyz";
        private static readonly short[] _BASE32_REV;

        private static readonly string[][] _NEIGHBORS =
        {
            new[]
            {
                "p0r21436x8zb9dcf5h7kjnmqesgutwvy", // North
                "bc01fg45238967deuvhjyznpkmstqrwx", // East
                "14365h7k9dcfesgujnmqp0r2twvyx8zb", // South
                "238967debc01fg45kmstqrwxuvhjyznp" // West
            },
            new[]
            {
                "bc01fg45238967deuvhjyznpkmstqrwx", // North
                "p0r21436x8zb9dcf5h7kjnmqesgutwvy", // East
                "238967debc01fg45kmstqrwxuvhjyznp", // South
                "14365h7k9dcfesgujnmqp0r2twvyx8zb" // West
            }
        };

        private static readonly string[][] _BORDERS =
        {
            new[] {"prxz", "bcfguvyz", "028b", "0145hjnp"},
            new[] {"bcfguvyz", "prxz", "0145hjnp", "028b"}
        };

        private static readonly double[] _CELLSIZES =
        {
            double.MaxValue,
            5000000,
            1250000,
            156000,
            39100,
            4890,
            1220,
            153,
            38.2,
            4.77,
            1.19,
            0.149,
            0.00372
        };


        static Geohash()
        {
            _BASE32_REV = new short['z' + 1];

            for (int i = 0; i < _BASE32.Length; i++)
                _BASE32_REV[(int) _BASE32[i]] = (short) i;
        }


        private static string Adjacent(string hash, Direction direction)
        {
            var len = hash.Length;

            char lastChr = hash[len - 1];
            int type = len%2;

            string adjacent = hash.Substring(0, len - 1);

            if (_BORDERS[type][(int) direction].IndexOf(lastChr) != -1)
            {
                adjacent = Adjacent(adjacent, direction);
            }

            return adjacent + _BASE32[_NEIGHBORS[type][(int) direction].IndexOf(lastChr)];
        }


        public static string[] Neighbors(string geohash)
        {
            return new[]
            {
                Adjacent(geohash, Direction.North),
                Adjacent(Adjacent(geohash, Direction.North), Direction.East),
                Adjacent(geohash, Direction.East),
                Adjacent(Adjacent(geohash, Direction.South), Direction.East),
                Adjacent(geohash, Direction.South),
                Adjacent(Adjacent(geohash, Direction.South), Direction.West),
                Adjacent(geohash, Direction.West),
                Adjacent(Adjacent(geohash, Direction.North), Direction.West)
            };
        }

        private static void RefineInterval(ref double[] interval, short cd, short mask)
        {
            if ((cd & mask) != 0)
            {
                interval[0] = (interval[0] + interval[1])/2;
            }
            else
            {
                interval[1] = (interval[0] + interval[1])/2;
            }
        }

        public static LatLon Decode(string geohash)
        {
            bool even = true;
            double[] lat = {-90.0, 90.0};
            double[] lon = {-180.0, 180.0};

            foreach (char c in geohash)
            {
                short cd = _BASE32_REV[c];

                for (int i = 0; i < 5; i++)
                {
                    if (even)
                        RefineInterval(ref lon, cd, (short)(1 << (4 - i)));
                    else
                        RefineInterval(ref lat, cd, (short)(1 << (4 - i)));

                    even = !even;
                }
            }

            return new LatLon((lat[0] + lat[1])/2, (lon[0] + lon[1])/2);
        }

        public static string Encode(LatLon latlon, int precision = 12)
        {
            return Encode(latlon.Lat, latlon.Lon, precision);
        }

        public static string Encode(double latitude, double longitude, int precision = 12)
        {
            precision = Math.Max(1, Math.Min(12, precision));

            StringBuilder geohash = new StringBuilder(precision);

            bool even = true;
            short bit = 0;
            short c = 0;

            double lat1 = -90.0, lat2 = 90.0;
            double lon1 = -180.0, lon2 = 180.0;

            while (geohash.Length < precision)
            {
                if (even)
                {
                    var mid = (lon1 + lon2)/2;

                    if (longitude > mid)
                    {
                        c |= (short)(1 << (4 - bit));
                        lon1 = mid;
                    }
                    else
                        lon2 = mid;
                }
                else
                {
                    var mid = (lat1 + lat2)/2;

                    if (latitude > mid)
                    {
                        c |= (short)(1 << (4 - bit));
                        lat1 = mid;
                    }
                    else
                        lat2 = mid;
                }

                even = !even;

                if (bit >= 4)
                {
                    geohash.Append(_BASE32[c]);
                    bit = 0;
                    c = 0;
                }
                else
                {
                    bit++;
                }
            }

            return geohash.ToString();
        }


        public static double CellSize(int precision)
        {
            if (precision > 12 || precision < 1)
                throw new ArgumentOutOfRangeException(nameof(precision), precision, "precision should be between 1 and 12");

            return _CELLSIZES[precision];
        }

        public static string[] PerimeterHashes(string centerHash, int distanceMeters)
        {
            int precision = 1;

            for (int i=1;i<_CELLSIZES.Length;i++)
                if (_CELLSIZES[i] < distanceMeters)
                {
                    precision = i - 1;
                    break;
                }
            
            return Neighbors(centerHash.Left(precision));
        }

        public static string[] PerimeterHashes(string centerHash, NumberWithUnit distance)
        {
            return PerimeterHashes(centerHash, (int) distance.To(Unit.Meters));
        }

    }
}
