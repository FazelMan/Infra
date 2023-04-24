using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Infra.Shared.Extensions;
using Infra.Shared.Dtos.Shared;

namespace Infra.Shared.Helpers
{
  
    public static class EnumHelpers
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        public static bool TryParse<TEnum>(int enumValue, out TEnum result)
            where TEnum : struct, IConvertible, IComparable
        {
            result = default(TEnum);

            if (!Enum.IsDefined(typeof(TEnum), enumValue))
                return false;

            result = (TEnum) (object) enumValue;

            return true;
        }

        public static string GetEnumLabel<T>(T key, string value) where T : struct, IConvertible, IComparable
        {
            if (string.IsNullOrWhiteSpace(value) || EqualityComparer<T>.Default.Equals(key, default(T)))
                return string.Empty;

            string label = value.InsertSpace();

            var fieldInfo = key.GetType().GetField(key.ToString());

            if (fieldInfo != null)
            {
                var descriptionAttributes = fieldInfo.GetCustomAttributes(
                    typeof(DisplayAttribute), false) as DisplayAttribute[];

                if (!descriptionAttributes.IsNullOrEmpty())
                {
                    if (!string.IsNullOrEmpty(descriptionAttributes[0].Name))
                        label = descriptionAttributes[0].Name;
                }
            }

            return label;
        }

        public static string GetEnumLabel<T>(string key, string value) where T : struct, IConvertible, IComparable
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string label = value.InsertSpace();

            var fieldInfo = key.GetType().GetField(key);

            if (fieldInfo != null)
            {
                var descriptionAttributes = fieldInfo.GetCustomAttributes(
                    typeof(DisplayAttribute), false) as DisplayAttribute[];

                if (!descriptionAttributes.IsNullOrEmpty())
                {
                    if (!string.IsNullOrEmpty(descriptionAttributes[0].Name))
                        label = descriptionAttributes[0].Name;
                }
            }

            return label;
        }

        public static IEnumerable<T> ConvertShortToArrayOfEnum<T>(int value) where T : struct, IConvertible
        {
            var result = new List<T>();
            foreach (var item in GetEnumValues<T>())
                if ((value & (int) (object) item) != 0)
                    result.Add(item);

            return result;
        }

        public static IEnumerable<T> ConvertShortToArrayOfEnum<T>(short value) where T : struct, IConvertible
        {
            var result = new List<T>();
            foreach (var item in GetEnumValues<T>())
                if ((value & (short) (object) item) != 0)
                    result.Add(item);

            return result;
        }

        public static short ConvertArrayOfEnumToShort<T>(IEnumerable<T> array)
        {
            if (array == null)
                return 0;
            return Convert.ToInt16(array.Aggregate(0, (result, item) => result | (short) (object) item));
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static bool StatusHasValue<T>(int status, T value) where T : struct, IConvertible
        {
            return (status & (int) (object) value) > 0;
        }

        public static IEnumerable<KeyValueDto<string, int>> GetEnumKeyValues<T>()
        {
            foreach (T obj in Enum.GetValues(typeof(T)))
            {
                Enum @enum = Enum.Parse(typeof(T), obj.ToString()) as Enum;
                int value = Convert.ToInt32(@enum);

                yield return new KeyValueDto<string, int>()
                {
                    Key = obj.ToString(),
                    Value = value
                };
            }
        }
    }
}