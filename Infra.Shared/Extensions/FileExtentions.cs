using Infra.Shared.Enums;
using Infra.Shared.Exceptions;

namespace Infra.Shared.Extensions
{
    public static class FileExtensions
    {
        public static string GetContentType(this FileExtensionType fileExtension)
        {
            var contentType = "";
            switch (fileExtension)
            {
                case FileExtensionType.Jpg:
                    return "image/jpeg";
                case FileExtensionType.Png:
                    return "image/png";
                case FileExtensionType.Gif:
                    return "image/gif";
                case FileExtensionType.Svg:
                    return "image/svg+xml";

                default:
                    throw new NotFoundException();
            }

            throw new BadRequestException("Extension {fileExtension} does not support!");
        }

        public static FileExtensionType GetFileExtension(this string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return FileExtensionType.Png;
                case "/9J/4":
                    return FileExtensionType.Jpg;
                case "AAAAF":
                    return FileExtensionType.Mp4;
                case "JVBER":
                    return FileExtensionType.Pdf;
                case "AAABA":
                    return FileExtensionType.Ico;
                case "UMFYI":
                    return FileExtensionType.Rar;
                case "E1XYD":
                    return FileExtensionType.Rtf;
                case "R0LGO":
                    return FileExtensionType.Gif;
                case "PHN2Z":
                    return FileExtensionType.Svg;
                case "U1PKC":
                    return FileExtensionType.Txt;
                case "MQOWM":
                case "77U/M":
                    return FileExtensionType.Srt;
                default:
                    throw new NotFoundException();
            }
        }
    }
}