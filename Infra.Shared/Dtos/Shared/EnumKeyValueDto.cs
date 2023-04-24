using System;
using Infra.Shared.Extensions;
using DisplayAttribute = System.ComponentModel.DataAnnotations.DisplayAttribute;

namespace Infra.Shared.Dtos.Shared;

public class EnumKeyValueDto<T> where T : struct, IConvertible, IComparable
{
    private string _label = null;

    public T? Key { get; set; }
    public string Value { get; set; }
    public string Label
    {
        get
        {
            if (string.IsNullOrEmpty(_label))
                _label = GetLabel();

            return _label;
        }
        set => _label = value;
    }

    public static EnumKeyValueDto<T> ToDto(T? key)
    {
        if (key == null)
            return null;

        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        return new EnumKeyValueDto<T>
        {
            Key = key,
            Value = key.ToString()
        };
    }

    public string GetLabel()
    {
        if (string.IsNullOrWhiteSpace(Value) || !Key.HasValue)
            return string.Empty;

        string label = Value.InsertSpace();

        var fieldInfo = Key.GetType().GetField(Key.ToString());

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
}