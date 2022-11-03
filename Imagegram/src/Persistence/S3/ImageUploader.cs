using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;

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
        
        public async Task UploadFileAsync(Stream imageToUpload, string imageName, CancellationToken ct)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            
            await fileTransferUtility.UploadAsync(imageToUpload,
                bucketName, imageName, ct);
        }
    }
}