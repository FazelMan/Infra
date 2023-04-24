using System;
using System.Collections;
using System.Reflection;

namespace Infra.Shared.Extensions
{
    public static class PropertyInfoExtension
    {
        public static PropertyInfo[] GetPropertyInfos<T>()
        {
            Type type = typeof(T);
            return type.GetProperties();
        }
        public static bool IsCollection(this PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(pi.PropertyType);
        }

        public static T[] GetAttributes<T>(this PropertyInfo pi) where T : Attribute
        {
            return (T[])pi.GetCustomAttributes(typeof(T), false);
        }
    }
}
