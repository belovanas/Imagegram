using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Imagegram.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S3;

namespace Imagegram.Controllers
{
    [Authorize]
    [Route("images")]
    public class ImageController : ControllerBase
    {
        private readonly IImageUploader _imageUploader;

        public ImageController(IImageUploader imageUploader)
        {
            _imageUploader = imageUploader;
        }

        [HttpPost]
        public async Task<ActionResult> AddForPost(ImageAddRequest request, CancellationToken ct)
        {
            await _imageUploader.UploadFile(request.Image.OpenReadStream(), request.PostId, ct);
            return Ok();
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetById([FromRoute] string postId, CancellationToken ct)
        {
            var image = await _imageUploader.GetFile(postId, ct);
            var mimeType = "image/jpeg";
            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                Inline = true // false = prompt the user for downloading; true = browser to try to show the file inline
            }.ToString());

            return File(image, mimeType, $"{postId}.jpeg");
        }
    }
}