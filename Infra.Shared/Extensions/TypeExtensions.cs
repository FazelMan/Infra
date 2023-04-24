using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infra.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsLikePrimitive(this Type typeToCheck)
        {
            if (
                typeToCheck.IsPrimitive ||
                typeToCheck == typeof(string) ||
                typeToCheck == typeof(decimal))
            {
                return true;
            }

            return false;
        }

        public static bool IsLikePrimitiveWithNullableAndDateTime(this Type typeToCheck)
        {
            if (Nullable.GetUnderlyingType(typeToCheck) != null)
                typeToCheck = Nullable.GetUnderlyingType(typeToCheck);

            if (
                typeToCheck.IsPrimitive ||
                typeToCheck == typeof(DateTime) ||
                typeToCheck == typeof(string) ||
                typeToCheck == typeof(decimal))
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<MethodInfo> GetMethodsBySig(this Type type, string name, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods().Where((m) =>
            {
                if (m.Name != name) return false;
                if (m.ReturnType != returnType) return false;

                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0))
                    return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
        }
    }
}
