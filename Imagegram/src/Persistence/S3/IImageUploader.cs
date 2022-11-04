using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace S3
{
    public interface IImageUploader
    {
        Task UploadFile(Stream fileToUpload, string fileKey, CancellationToken ct);
        Task DeleteFile(string fileKey, CancellationToken ct);
        Task<Stream> GetFile(string fileKey, CancellationToken ct);
        string GetLinkToFile(string fileKey);
    }
}