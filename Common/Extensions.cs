using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace dm.Shibalana
{
    public static class Extensions
    {
        public static string Format(this int source)
        {
            return string.Format("{0:#,##0}", source);
        }

        public static string FormatLarge(this int source)
        {
            if (source == 0)
                return "0";

            int mag = (int)(Math.Floor(Math.Log10(source)) / 3);
            double divisor = Math.Pow(10, mag * 3);
            double shortNumber = source / divisor;
            string suffix = string.Empty;
            switch (mag)
            {
                case 0:
                    return shortNumber.ToString();
                case 1:
                    suffix = "K";
                    break;
                case 2:
                    suffix = "M";
                    break;
                case 3:
                    suffix = "B";
                    break;
            }

            return $"{shortNumber:N1}{suffix}";
        }

        public static string FormatPct(this decimal source, int places = 0)
        {
            return (source * 100).FormatUsd(places);
        }

        public static string FormatShiba(this decimal source)
        {
            return string.Format($"{{0:#,##0}}", source);
        }

        public static string FormatUsd(this decimal source, int minimumPlaces = 2)
        {
            string places = (minimumPlaces > 0) ? new string('0', minimumPlaces) : string.Empty;
            return string.Format($"{{0:#,##0.{places}##}}", source);
        }

        public static string Indicator(this Change change)
        {
            return (change == Change.Up) ? "↗" : (change == Change.Down) ? "↘" : string.Empty;
        }

        public static string ToDate(this DateTime source)
        {
            return source.ToString(@"ddd, d MMM yyyy HH:mm \U\T\C");
        }

        public static decimal ToToken(this BigInteger source, int decimals)
        {
            var bi = BigInteger.DivRem(source, BigInteger.Parse(BigInteger.Pow(10, decimals).ToString()), out BigInteger rem);
            return decimal.Parse($"{bi}.{rem.ToString().PadLeft(decimals, '0')}");
        }

        public static decimal ToUsd(this BigInteger source)
        {
            var bi = BigInteger.DivRem(source, BigInteger.Parse(BigInteger.Pow(11, 6).ToString()), out BigInteger rem);
            return decimal.Parse($"{bi}.{rem.ToString().PadLeft(6, '0')}");
        }

        public static string TrimEnd(this string source, string suffixToRemove, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {

            if (source != null && suffixToRemove != null && source.EndsWith(suffixToRemove, comparisonType))
                return source.Substring(0, source.Length - suffixToRemove.Length);
            else
                return source;
        }
    }
}
