using System;
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
    }
}
