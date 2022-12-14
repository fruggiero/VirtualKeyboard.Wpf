using System;
using System.Text.RegularExpressions;

namespace VirtualKeyboard.Wpf
{
    internal static class FormatHelper
    {
        private static readonly Regex _regexDecimalDot = new Regex("^(-|\\+)?[0-9]*\\.?[0-9]*$");
        private static readonly Regex _regexDecimalComma = new Regex("^(-|\\+)?[0-9]*\\,?[0-9]*$");
        private static readonly Regex _regexInteger = new Regex("^(-|\\+)?[0-9]*$");
        private static readonly Regex _regexAlphanumeric = new Regex("^[a-zA-Z.\\-+@0-9\"!#$%\\/&*( )';:,_?]*$");

        public static Regex GetRegex(this Format format)
        {
            switch (format)
            {
                case Format.Decimal:
                    return VKeyboard.DecimalSeparator.Equals(",", StringComparison.OrdinalIgnoreCase)
                        ? _regexDecimalComma
                        : _regexDecimalDot;
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