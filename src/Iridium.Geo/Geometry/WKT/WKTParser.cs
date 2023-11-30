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

        private WKTToken _currentToken;

        private void NextToken()
        {
            _currentToken = _tokenizer.NextToken();
        }

        private void NextToken(WKTTokenType expected)
        {
            if (expected != _currentToken.Type)
                throw new Exception(expected + " expected");

            NextToken();
          
        }

        private IGeometry ParseGeometry()
        {
            switch (_currentToken.Type)
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

                if (_currentToken.Type == WKTTokenType.Comma)
                {
                    NextToken();
                    continue;
                }

                if (_currentToken.Type == WKTTokenType.Close)
                    return points;

                throw new Exception(") expected for linestring");
            }
        }

        private Poly ParseLineString()
        {
            NextToken(WKTTokenType.LineString);
            NextToken(WKTTokenType.Open);

            var polygon = new Polyline(ParsePoints());

            NextToken(WKTTokenType.Close);

            return polygon;
        }

        private MultiPoly ParseMultiLineString()
        {
            NextToken(WKTTokenType.MultilineString);

            NextToken(WKTTokenType.Open);

            List<Polyline> polylines = new List<Polyline>();

            for (;;)
            {
                if (_currentToken.Type == WKTTokenType.Close)
                    break;

                NextToken(WKTTokenType.Open);

                polylines.Add(new Polyline(ParsePoints()));

                NextToken(WKTTokenType.Close);

                if (_currentToken.Type == WKTTokenType.Comma)
                    NextToken();
            }

            NextToken();

            return new MultiPoly(polylines);
        }

        private MultiPoint ParseMultiPoint()
        {
            NextToken(WKTTokenType.MultiPoint);
            NextToken(WKTTokenType.Open);

            List<Point> points = new List<Point>();

            for (;;)
            {
                if (_currentToken.Type == WKTTokenType.Close)
                    break;

                if (_currentToken.Type == WKTTokenType.Open)
                {
                    NextToken();

                    points.Add(ParsePointNumbers());

                    NextToken(WKTTokenType.Close);
                }
                else
                {
                    points.Add(ParsePointNumbers());
                }

                if (_currentToken.Type == WKTTokenType.Comma)
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
            if (_currentToken.Type != WKTTokenType.Number)
                throw new Exception("number expected");

            double n = _currentToken.Number;

            NextToken();

            return n;
        }


    }
}
