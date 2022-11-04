using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain;
using Imagegram.Dto;
using Imagegram.Extensions;
using Imagegram.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [Authorize]
    [Route("post")]
    public class PostController : ControllerBase
    {
        private const long MaxFileSize = 100L * 1024L * 1024L; // 100Mb

        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostController(
            IPostService postService, 
            ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [HttpPost]
        public async Task Create(PostCreateRequest postCreateRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            var post = new Post()
            {
                Info = new PostInfo()
                {
                    User = user,
                    Caption = postCreateRequest.Caption
                }
            };
            var image = postCreateRequest.Image.OpenReadStream();
            await _postService.Add(post, image, ct);
        }
        
        [HttpPost("{id}/comment")]
        public async Task AddComment([FromRoute] string id, [FromBody] CommentCreateRequest commentRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            var comment = new Comment()
            {
                PostId = id,
                Content = commentRequest.Content,
                User = user
            };
            await _commentService.Add(comment, ct);
        }
        
        [HttpDelete("{id}/comment/{commentId}")]
        public async Task AddComment([FromRoute] string id, [FromRoute] string commentId, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            await _commentService.Delete(commentId, id, user, ct);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetByIdV2([FromRoute] string id, CancellationToken ct)
        {
            var post = await _postService.GetById(id, ct);
           
            var postDto = new PostDto
            {
                Id = post.Info.Id,
                User = post.Info.User,
                Caption = post.Info.Caption,
                CreatedAt = post.Info.CreatedAt,
                Comments = post.Comments.Select(comment => new CommentDto()
                {
                    Id = comment.Id,
                    User = comment.User,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
            return Ok(postDto);
        }
        
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetImageById([FromRoute] string id, CancellationToken ct)
        {
            var image = await _postService.GetImageForPost(id, ct);
            var mimeType="image/jpeg";
            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                Inline = true // false = prompt the user for downloading; true = browser to try to show the file inline
            }.ToString());

            return File(image, mimeType);
        }
    }
}