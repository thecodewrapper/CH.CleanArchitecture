using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Common
{
    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value) where T : struct {
            if (!Enum.TryParse<T>(value, out var enumeration)) {
                return default;
            }
            return enumeration;
        }

        public static MarkupString ToMarkupString(this string value) {
            return new MarkupString(value);
        }

        public static string ToInitials(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return string.Empty;
            }

            var builder = new StringBuilder();

            var words = value.Split(" ");
            foreach (var word in words) {
                builder.Append(word.Substring(0, 1));
            }

            return builder.ToString().ToUpper();
        }

        public static string TrimStart(this string target, string trimString) {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString)) {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string Base64UrlEncode(this string value) {
            var bytes = Encoding.UTF8.GetBytes(value);
            var s = Convert.ToBase64String(bytes); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        public static string Base64UrlDecode(this string value) {
            var s = value;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    s += "==";
                    break; // Two pad chars
                case 3:
                    s += "=";
                    break; // One pad char
                default:
                    throw new Exception("Illegal base64 url string!");
            }

            var bytes = Convert.FromBase64String(s); // Standard base64 decoder
            return Encoding.UTF8.GetString(bytes);
        }

        public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => input,
            "" => input,
            _ => input.First().ToString().ToUpper() + input.Substring(1)
        };
    }
}
