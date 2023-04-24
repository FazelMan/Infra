using System.Text.Json.Serialization;

namespace Infra.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FileExtensionType
    {
        Jpg = 1,
        Png = 2,
        Svg = 3,
        Gif = 4,
        Mp4 = 5,
        Pdf = 6,
        Ico = 7,
        Rar = 8,
        Rtf = 9,
        Txt = 10,
        Srt = 11
    }
}