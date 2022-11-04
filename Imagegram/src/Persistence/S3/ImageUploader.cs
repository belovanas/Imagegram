using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace S3
{
    public class ImageUploader : IImageUploader
    {
        private const string bucketNameToUpload = "imagegram-bandlab";
        private const string bucketNameToGet = "imagegram-bandlab-resized";
        private static IAmazonS3 _s3Client;

        public ImageUploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task UploadFile(Stream fileToUpload, string fileKey, CancellationToken ct)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            await fileTransferUtility.UploadAsync(fileToUpload,
                bucketNameToUpload, fileKey, ct);
        }

        public async Task DeleteFile(string fileKey, CancellationToken ct)
        {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucketNameToUpload,
                Key = fileKey
            }, ct);
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucketNameToGet,
                Key = fileKey
            }, ct);
        }

        public string GetLinkToFile(string fileKey)
        {
            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucketNameToGet,
                Key = fileKey,
                ContentType = "image/jpeg",
                Expires = DateTime.Now.AddHours(1)
            });
        }

        public async Task<Stream> GetFile(string fileKey, CancellationToken ct)
        {
            var responseHeaders = new ResponseHeaderOverrides
            {
                CacheControl = "No-cache",
                ContentType = "image/jpeg"
            };
            var request = new GetObjectRequest
            {
                BucketName = bucketNameToGet,
                Key = fileKey,
                ResponseHeaderOverrides = responseHeaders
            };
            using var getObjectResponse = await _s3Client.GetObjectAsync(request, ct);
            await using var responseStream = getObjectResponse.ResponseStream;
            var stream = new MemoryStream();
            await responseStream.CopyToAsync(stream, ct);
            stream.Position = 0;
            return stream;
        }
    }
}