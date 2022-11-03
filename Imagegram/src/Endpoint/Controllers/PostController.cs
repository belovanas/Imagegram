using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Imagegram.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using S3;

namespace Imagegram.Controllers
{
    [Authorize]
    [Route("post")]
    public class PostController : ControllerBase
    {
        private const long MaxFileSize = 100L * 1024L * 1024L; // 100Mb

        private readonly IImageUploader _imageUploader;

        public PostController(IImageUploader imageUploader)
        {
            _imageUploader = imageUploader;
        }
        
        [DisableFormValueModelBinding]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [HttpPost]
        public async Task ReceiveFile(IFormFile file, CancellationToken ct)
        {
            var stream = file.OpenReadStream();
            await _imageUploader.UploadFileAsync(stream, file.Name, ct);
        }
    }
}