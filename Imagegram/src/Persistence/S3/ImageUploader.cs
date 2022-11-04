using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace S3
{
    public class ImageUploader : IImageUploader
    {
        private const string bucketName = "imagegram-bandlab";
        private static IAmazonS3 _s3Client;

        public ImageUploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        
        public async Task UploadFile(Stream fileToUpload, string fileKey, CancellationToken ct)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            
            await fileTransferUtility.UploadAsync(fileToUpload,
                bucketName, fileKey, ct);
        }
        
        public async Task<Stream> GetFile(string fileKey, CancellationToken ct)
        {
            var responseHeaders = new ResponseHeaderOverrides()
            {
                CacheControl = "No-cache",
                ContentType = "image/jpeg"
            };
            var request = new GetObjectRequest()
            {
                BucketName = bucketName,
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