using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dissimilis.Core.Types
{
    public static class StringExtensions
    {
        /// <summary>
        /// Makes the whole string ToLower except the first letter
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            s = s.ToLower();
            var a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        [DebuggerStepThrough]
        public static bool IsEqual(this string a, string b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            return a.Equals(b, comparisonType: StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsHtml(this string value)
        {
            return Regex.IsMatch(value, "<(.|\n)*?>");
        }

        public static string ReplaceHtmlEntities(this string value)
        {
            var replaceChars = new[]
            {
                new {Entity = "&aring;", ReplaceValue = "å"},
                new {Entity = "&oslash;", ReplaceValue = "ø"}
            };

            if (string.IsNullOrWhiteSpace(value) || !value.ContainsHtml())
            {
                return value;
            }

            return replaceChars
                .Aggregate(value,
                    (current, replaceChar) => current.Replace(replaceChar.Entity, replaceChar.Entity)
                );
        }

        [DebuggerStepThrough]
        public static string CommaJoin(this string a, string b) =>
            string.Join(", ", new[] { a, b }.Where(s => !string.IsNullOrWhiteSpace(s)));

        [DebuggerStepThrough]
        public static string CommaJoin(this IEnumerable<string> strings) =>
            string.Join(", ", strings.Where(s => !string.IsNullOrWhiteSpace(s)));

        [DebuggerStepThrough]
        public static string CommaJoin(this IEnumerable<int> numbers) =>
            string.Join(", ", numbers);

        [DebuggerStepThrough]
        public static string NullToEmpty(this string s)
        {
            return s.IsNullOrWhiteSpace()
                ? string.Empty
                : s.Trim();
        }

        [DebuggerStepThrough]
        public static string EmptyToNull(this string s)
        {
            if (s == null)
            {
                return null;
            }

            var trimmedS = s.Trim();

            if (trimmedS.Equals(string.Empty))
            {
                return null;
            }

            return trimmedS;
        }



        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string s) =>
            string.IsNullOrWhiteSpace(s);

        [DebuggerStepThrough]
        public static bool NotNullOrWhiteSpace(this string s) =>
            !string.IsNullOrWhiteSpace(s);

        [DebuggerStepThrough]
        public static string TrimAllSpaces(this string s) =>
            s.IsNullOrWhiteSpace()
                ? null
                : s.Replace(" ", "").Trim();

        [DebuggerStepThrough]
        public static bool EqualsByContent(this string s1, string s2)
        {
            if (!string.IsNullOrWhiteSpace(s1) && !string.IsNullOrWhiteSpace(s2))
            {
                return s1.Trim().Equals(s2.Trim(), StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public static bool EqualsTrimmed(this string s1, string s2)
        {
            return string.Equals(
                string.IsNullOrWhiteSpace(s1) ? string.Empty : s1.Trim(),
                string.IsNullOrWhiteSpace(s2) ? string.Empty : s2.Trim());
        }

        public static string MergePlaceholders(this string text, Dictionary<string, string> paceholders)
        {
            foreach (var placeholder in paceholders)
            {
                text = text.Replace($"[{placeholder.Key}]", placeholder.Value);
            }
            return text;
        }

        /// <summary>
        /// Following rules given:
        /// https://www.nkom.no/telefoni-og-telefonnummer/telefonnummer-og-den-norske-nummerplan/alle-nummerserier-for-norske-telefonnumre
        ///
        /// There is also traces of concatinated numbers in the database, trying to deal with this as well
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetValidMobileNumber(this string text)
        {
            string GetValidNumberAmongAlternatives(string[] alternatives)
            {
                return alternatives
                    .Select(number => number.TrimStart("+47".ToCharArray()).TrimStart("0047".ToCharArray()))
                    .FirstOrDefault(number => number.StartsWith("4") || number.StartsWith("9"));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Contains('-'))
            {
                text = GetValidNumberAmongAlternatives(text.Split('-'));
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Contains('|'))
            {
                text = GetValidNumberAmongAlternatives(text.Split('|'));
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Contains(';'))
            {
                text = GetValidNumberAmongAlternatives(text.Split(';'));
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Contains(','))
            {
                text = GetValidNumberAmongAlternatives(text.Split(','));
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Length > 8)
            {
                text = text.TrimStart("+47".ToCharArray());
                text = text.TrimStart("0047".ToCharArray());
            }

            if (!string.IsNullOrWhiteSpace(text) && text.Length == 8 && (text.StartsWith("4") || text.StartsWith("9")))
            {
                return text;
            }

            return null;
        }

        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string StreamToString(this Stream stream)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
