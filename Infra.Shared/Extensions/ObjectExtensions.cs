using System;

namespace Infra.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static bool TryChangeType<T>(this object source, out T value)
        {
            try
            {
                Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                value = (T)Convert.ChangeType(source, type);
                return true;

            }
            catch
            {

                value = default(T);
                return false;

            }

        }
    }

}
