using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Iridium.Geo
{
    internal class WKTTokenizer
    {
        public WKTTokenizer(string s)
        {
            _charFeeder = new StringCharFeeder(s);
        }

        public WKTTokenizer(Stream stream)
        {
            _charFeeder = new StreamCharFeeder(stream);
        }

        public abstract class CharFeeder
        {
            private bool _backTracked = false;
            private char _current;

            public char Current => _current;

            protected abstract int ReadNext();

            public char Next()
            {
                if (_backTracked)
                {
                    _backTracked = false;
                }
                else
                {
                    int next = ReadNext();

                    if (next < 0)
                        _current = (char)0;
                    else
                        _current = (char)next;
                }

                return _current;
            }

            public void Backtrack()
            {
                _backTracked = true;
            }
        }

        public class StringCharFeeder : CharFeeder
        {
            private string _s;
            private int _index;

            public StringCharFeeder(string s)
            {
                _s = s;
                _index = 0;
            }

            protected override int ReadNext()
            {
                return _index >= _s.Length ? -1 : _s[_index++];
            }
        }

        public class StreamCharFeeder : CharFeeder
        {
            private readonly StreamReader _reader;

            public StreamCharFeeder(Stream stream)
            {
                _reader = new StreamReader(stream, Encoding.UTF8);
            }

            protected override int ReadNext()
            {
                return _reader.Read();
            }
        }


        private readonly CharFeeder _charFeeder;

        private static readonly Dictionary<char, WKTTokenType> _tokenTypes = new Dictionary<char, WKTTokenType>()
        {
            {'(', WKTTokenType.Open},
            {')', WKTTokenType.Close},
            {',',WKTTokenType.Comma },
        };

        private static readonly Dictionary<string, WKTTokenType> _keywords = new Dictionary<string, WKTTokenType>()
        {
            {"MULTILINESTRING", WKTTokenType.MultilineString},
            {"LINESTRING", WKTTokenType.LineString},
            {"POINT", WKTTokenType.Point},
            {"MULTIPOINT",WKTTokenType.MultiPoint }
        };

        public WKTToken NextToken()
        {
            for (;;)
            {
                char c = _charFeeder.Next();

                if (char.IsWhiteSpace(c))
                    continue;

                if (_tokenTypes.TryGetValue(c, out var tokenType))
                {
                    return new WKTToken(tokenType);
                }
                if (char.IsDigit(c) || c == '-')
                {
                    return ReadNumber();
                }
                if (char.IsLetter(c))
                {
                    return ReadKeyword();
                }

                return new WKTToken(WKTTokenType.EOF);
            }
        }

        private WKTToken ReadKeyword()
        {
            char c = _charFeeder.Current;

            string keyword = c.ToString();

            for (;;)
            {
                if (_keywords.TryGetValue(keyword, out var match))
                {
                    return new WKTToken(match);
                }

                c = _charFeeder.Next();

                if (!char.IsLetter(c))
                    throw new Exception($"Invalid keyword {keyword}");

                keyword += c;
            }
        }

        private WKTToken ReadNumber()
        {
            bool hasDot = false;
            string s = new string(_charFeeder.Current, 1);

            for (;;)
            {
                char c = _charFeeder.Next();

                if (c == '.' && !hasDot)
                {
                    hasDot = true;
                    s += c;
                }
                else if (char.IsDigit(c))
                {
                    s += c;
                }
                else
                {
                    _charFeeder.Backtrack();

                    return new WKTToken( double.Parse(s));
                }
            }
        }


    }
}