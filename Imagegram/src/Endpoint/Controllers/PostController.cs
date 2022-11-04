using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain;
using Imagegram.Dto;
using Imagegram.Extensions;
using Imagegram.Mapping;
using Imagegram.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [Authorize]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private const long MaxFileSize = 100L * 1024L * 1024L; // 100Mb
        private readonly ICommentService _commentService;

        private readonly IPostService _postService;

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
        public async Task<ActionResult<string>> Create(PostCreateRequest postCreateRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();

            var post = new Post
            {
                User = user,
                Caption = postCreateRequest.Caption
            };
            await _postService.Create(post, postCreateRequest.Image.OpenReadStream(), ct);
            return Ok(post.Id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id, CancellationToken ct)
        {
            await _postService.Delete(id, ct);
            return Ok();
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult<string>> AddComment([FromRoute] string id,
            [FromBody] CommentCreateRequest commentRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            var comment = new Comment
            {
                PostId = id,
                Content = commentRequest.Content,
                User = user
            };
            await _commentService.Add(comment, ct);
            return Ok(comment.Id);
        }

        [HttpDelete("{id}/comment/{commentId}")]
        public async Task<ActionResult> DeleteComment([FromRoute] string id, [FromRoute] string commentId,
            CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            await _commentService.Delete(commentId, id, user, ct);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetById([FromRoute] string id, CancellationToken ct)
        {
            var post = await _postService.Get(id, ct);
            var postDto = post.ToDto();
            return Ok(postDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<PostDto>>> GetAllPosts(
            CancellationToken ct,
            [FromQuery] [Required] int limit = 5,
            [FromQuery] [Required] int offset = 0)
        {
            var posts = await _postService.GetPage(limit, offset, ct);
            var postDtos = posts.Select(x => x.ToShortenedDto()).ToList();
            return postDtos;
        }
    }
}