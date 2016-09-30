using System;
using System.Collections.Generic;
using System.IO;

namespace Iridium.Geo
{
    public class WKTParser
    {
        public static IGeometry Parse(string s)
        {
            return new WKTParser(s)._Parse();
        }

        public static IGeometry Parse(Stream stream)
        {
            return new WKTParser(stream)._Parse();
        }

        public static T Parse<T>(string s) where T:class,IGeometry
        {
            return new WKTParser(s)._Parse() as T;
        }

        public static T Parse<T>(Stream stream) where T : class, IGeometry
        {
            return new WKTParser(stream)._Parse() as T;
        }

        private readonly WKTTokenizer _tokenizer;

        public WKTParser(string s)
        {
            _tokenizer = new WKTTokenizer(s);
        }

        public WKTParser(Stream stream)
        {
            _tokenizer = new WKTTokenizer(stream);
        }

        private IGeometry _Parse()
        {
            NextToken();

            return ParseGeometry();
        }

        private WKTToken CurrentToken;

        private void NextToken()
        {
            CurrentToken = _tokenizer.NextToken();
        }

        private void NextToken(WKTTokenType expected)
        {
            if (expected != CurrentToken.Type)
                throw new Exception(expected + " expected");

            NextToken();
          
        }

        private IGeometry ParseGeometry()
        {
            switch (CurrentToken.Type)
            {
                case WKTTokenType.Point:
                    return ParsePoint();

                case WKTTokenType.LineString:
                    return ParseLineString();

                case WKTTokenType.MultilineString:
                    return ParseMultiLineString();

                case WKTTokenType.MultiPoint:
                    return ParseMultiPoint();
            }


            return null;
        }

        private List<Point> ParsePoints()
        {
            var points = new List<Point>();

            for (;;)
            {
                points.Add(ParsePointNumbers());

                if (CurrentToken.Type == WKTTokenType.Comma)
                {
                    NextToken();
                    continue;
                }

                if (CurrentToken.Type == WKTTokenType.Close)
                    return points;

                throw new Exception(") expected for linestring");
            }
        }

        private Polygon ParseLineString()
        {
            NextToken(WKTTokenType.LineString);
            NextToken(WKTTokenType.Open);

            var polygon = new Polygon(ParsePoints());

            NextToken(WKTTokenType.Close);

            return polygon;
        }

        private MultiPolygon ParseMultiLineString()
        {
            NextToken(WKTTokenType.MultilineString);

            NextToken(WKTTokenType.Open);

            List<Polygon> polygons = new List<Polygon>();

            for (;;)
            {
                if (CurrentToken.Type == WKTTokenType.Close)
                    break;

                NextToken(WKTTokenType.Open);

                polygons.Add(new Polygon(ParsePoints()));

                NextToken(WKTTokenType.Close);

                if (CurrentToken.Type == WKTTokenType.Comma)
                    NextToken();
            }

            NextToken();

            return new MultiPolygon(polygons);
        }

        private MultiPoint ParseMultiPoint()
        {
            NextToken(WKTTokenType.MultiPoint);
            NextToken(WKTTokenType.Open);

            List<Point> points = new List<Point>();

            for (;;)
            {
                if (CurrentToken.Type == WKTTokenType.Close)
                    break;

                if (CurrentToken.Type == WKTTokenType.Open)
                {
                    NextToken();

                    points.Add(ParsePointNumbers());

                    NextToken(WKTTokenType.Close);
                }
                else
                {
                    points.Add(ParsePointNumbers());
                }

                if (CurrentToken.Type == WKTTokenType.Comma)
                    NextToken();

            }

            NextToken();

            return new MultiPoint(points);
        }

        private Point ParsePoint()
        {
            NextToken(WKTTokenType.Point);
            NextToken(WKTTokenType.Open);

            Point p = ParsePointNumbers();

            NextToken(WKTTokenType.Close);

            return p;
        }

        private Point ParsePointNumbers()
        {
            double x = ParseNumber();
            double y = ParseNumber();

            return new Point(x, y);
        }

        private double ParseNumber()
        {
            if (CurrentToken.Type != WKTTokenType.Number)
                throw new Exception("number expected");

            double n = CurrentToken.Number;

            NextToken();

            return n;
        }


    }
}
