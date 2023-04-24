using System.IO;
using System.Threading.Tasks;
using Infra.Shared.Enums;

namespace Infra.Shared.CloudBucket.Abstraction
{
    public interface ICloudBucket
    {
        string GetFileUrl(string fullName, string bucketPath);
        Task<Stream> GetFileStreamAsync(string bucketPath, string fullName);
        
        Task SaveFileAsync(string bucketPath, 
            string fileBase64, 
            string fileName, 
            FileExtensionType fileExtension = FileExtensionType.Jpg);
        
        Task SaveFileAsync(string bucketPath, 
            Stream stream,
            string fileName, 
            FileExtensionType fileExtension = FileExtensionType.Jpg);
        Task DeleteFileAsync(string bucketPath, string fullName);
    }
}