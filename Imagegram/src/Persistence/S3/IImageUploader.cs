using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace S3
{
    public interface IImageUploader
    {
        Task UploadFileAsync(Stream imageToUpload, string imageName, CancellationToken ct);
    }
}