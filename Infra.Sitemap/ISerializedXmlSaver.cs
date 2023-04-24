using System.IO;

namespace Infra.Sitemap
{
    
    internal interface ISerializedXmlSaver<in T>
    {
        FileInfo SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName);
    }
}