namespace Iridium.Geo
{
    internal class WKTToken
    {
        public WKTTokenType Type;
        public string Token;
        public double Number { get; }

        public WKTToken(WKTTokenType tokenType)
        {
            Type = tokenType;
        }

        public WKTToken(WKTTokenType tokenType, string s)
        {
            Type = tokenType;
            Token = s;
        }

        public WKTToken(double number)
        {
            Type = WKTTokenType.Number;
            Number = number;
        }
    }
}