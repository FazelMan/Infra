using System;
using System.Reflection;

namespace Infra.Shared.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static T GetAttribute<T>(this PropertyInfo prop, bool inherit = false) where T : Attribute
        {
            T result = default(T);

            var foundAttr = prop.GetCustomAttributes(typeof(T), inherit);

            if (foundAttr.Length > 0)
            {
                foundAttr.TryChangeType(out T[] resultArray);

                if (resultArray != default(T[]) && resultArray.Length > 0)
                    result = resultArray[0];
            }

            return result;
        }
    }

}
