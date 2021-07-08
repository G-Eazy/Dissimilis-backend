using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Dissimilis.Core.Collections
{
    public static class EnumExtensions
    {

        /// <summary>
        /// Use this in stead of Enum.TryParse if you expect that value can be numeric,
        /// as Enum.TryParse will wrongfully return true in any case.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string value, out T enumValue) where T : struct
        {
            if (Enum.TryParse<T>(value, true, out var triedValue))
            {
                if (GetEnumValues<T>().Any(t => t.Equals(triedValue)))
                {
                    enumValue = triedValue;
                    return true;
                }
            }

            enumValue = default;
            return false;
        }

        public static IEnumerable<T> GetEnumValues<T>() =>
            Enum.GetValues(typeof(T)).OfType<T>();


        public static string GetDescription<T>(this T status) where T : struct
        {
            FieldInfo fi = status.GetType().GetField(status.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return "";
        }
    }
}