using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Infra.Shared.Attributes;
using Infra.Shared.CloudBucket;
using Infra.Shared.CloudBucket.Abstraction;
using Infra.Shared.Enums;
using Infra.Shared.Extensions;

namespace Core.CloudBucket.Arvan
{
    [Name(Name)]
    public class ArvanCloudBucket : ICloudBucket
    {
        public const string Name = nameof(ArvanCloudBucket);

        protected readonly AmazonS3Client AmazonS3Client;

        public ArvanCloudBucket()
        {
            var awsAccessKeyId = Infra.Shared.Helpers.Host.Config["ArvanStorageSecret:AccessKey"];
            var awsSecretAccessKey = Infra.Shared.Helpers.Host.Config["ArvanStorageSecret:SecretKey"];
            var amazonS3Config = new AmazonS3Config
            {
                ServiceURL = Infra.Shared.Helpers.Host.Config["ArvanStorageSecret:Url"]
            };
            this.AmazonS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, amazonS3Config);
        }

        public string GetFileUrl(string fullName, string bucketPath)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            return
                $"{Infra.Shared.Helpers.Host.Config["ArvanStorageSecret:Url"]}/{bucketPath}/{fullName}";
        }

        public async Task<Stream> GetFileStreamAsync(string bucketPath, string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            var request = new GetObjectRequest();
            request.BucketName = bucketPath;
            request.Key = fullName;

            var result = await AmazonS3Client.GetObjectAsync(request);

            return result.ResponseStream;
        }

        public Task SaveFileAsync(string bucketPath, 
            string fileBase64,
            string fileName,
            FileExtensionType fileExtension = FileExtensionType.Jpg)
        {
            if (string.IsNullOrEmpty(fileBase64))
                return null;

            var stream = new MemoryStream(Convert.FromBase64String(fileBase64));
            var request = new PutObjectRequest
            {
                BucketName = bucketPath,
                Key = fileName,
                CannedACL = S3CannedACL.PublicRead,
                ContentType = fileExtension.GetContentType(),
                InputStream = stream
            };

            return AmazonS3Client.PutObjectAsync(request);
        }

        public Task SaveFileAsync(string bucketPath, 
            Stream stream,
            string fileName,
            FileExtensionType fileExtension = FileExtensionType.Jpg)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            var request = new PutObjectRequest();
            request.BucketName = bucketPath;
            request.Key = fileName;
            request.CannedACL = S3CannedACL.PublicRead;
            request.ContentType = fileExtension.GetContentType();
            request.InputStream = stream;

            return AmazonS3Client.PutObjectAsync(request);
        }

        public Task DeleteFileAsync(string bucketPath, string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            var request = new DeleteObjectRequest()
            {
                BucketName = bucketPath,
                Key = fullName
            };

            return this.AmazonS3Client.DeleteObjectAsync(request);
        }
    }
}