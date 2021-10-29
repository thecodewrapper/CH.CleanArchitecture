using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CH.CleanArchitecture.Common
{
    public static class EnumExtensions
    {
        public static List<T> ToList<T>(this T value) where T : Enum {
            return value.ToString().Split(',').Select(flag => (T)Enum.Parse(typeof(T), flag)).ToList();
        }

        public static string GetDisplayName<T>(this T enumValue) where T : Enum {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .GetName() ?? enumValue.ToString();
        }
    }
}
