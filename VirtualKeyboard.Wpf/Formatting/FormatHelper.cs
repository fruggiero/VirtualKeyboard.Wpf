using System.Text.RegularExpressions;

namespace VirtualKeyboard.Wpf
{
    internal static class FormatHelper
    {
        private static readonly Regex _regexDecimal = new Regex("^(-|\\+)?[0-9]*\\.?[0-9]*$");
        private static readonly Regex _regexInteger = new Regex("^(-|\\+)?[0-9]*$");
        private static readonly Regex _regexAlphanumeric = new Regex("^[a-zA-Z.\\-+@0-9\"!#$%\\/&*( )';:,_?]*$");

        public static Regex GetRegex(this Format format)
        {
            switch (format)
            {
                case Format.Decimal:
                    return _regexDecimal;
                case Format.Integer:
                    return _regexInteger;
                case Format.Alphanumeric:
                    return _regexAlphanumeric;
                default:
                    return null;
            }
        }
    }
}