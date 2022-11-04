using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain;
using Dynamo.Abstractions;
using Imagegram.Dto;
using Imagegram.Extensions;
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

        private readonly IPostRepository _postRepository;
        private readonly ICommentService _commentService;

        public PostController(
            IPostRepository postRepository, 
            ICommentService commentService)
        {
            _postRepository = postRepository;
            _commentService = commentService;
        }

        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [HttpPost]
        public async Task<ActionResult<string>> Create(PostCreateRequest postCreateRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            var post = new Post()
            {
                User = user,
                Caption = postCreateRequest.Caption
            };
            await _postRepository.Add(post, ct);
            return Ok(post.Id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id, CancellationToken ct)
        {
            await _postRepository.Delete(id, ct);
            return Ok();
        }
        
        [HttpPost("{id}/comment")]
        public async Task<ActionResult<string>> AddComment([FromRoute] string id, [FromBody] CommentCreateRequest commentRequest, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            var comment = new Comment()
            {
                PostId = id,
                Content = commentRequest.Content,
                User = user
            };
            await _commentService.Add(comment, ct);
            return Ok(comment.Id);
        }
        
        [HttpDelete("{id}/comment/{commentId}")]
        public async Task<ActionResult> DeleteComment([FromRoute] string id, [FromRoute] string commentId, CancellationToken ct)
        {
            var user = HttpContext.GetUser();
            await _commentService.Delete(commentId, id, user, ct);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetById([FromRoute] string id, CancellationToken ct)
        {
            var post = await _postRepository.Get(id, ct);
            var postDto = ToDto(post);
            return Ok(postDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<PostDto>>> GetAllPosts(
            CancellationToken ct, 
            [FromQuery] [Required] int limit = 5,
            [FromQuery] [Required] int offset = 0)
        {
            var posts = await _postRepository.GetAll(ct);
            var sortedPosts = posts.OrderByDescending(x => x.Comments.Count);
            var pagedPosts = sortedPosts.Skip(offset).Take(limit);
            var postDtos = pagedPosts.Select(ToShortenedDto).ToList();
            return postDtos;
        }

        private PostDto ToDto(Post post)
        {
            return new()
            {
                Id = post.Id,
                User = post.User,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                Comments = post.Comments.Select(comment => new CommentDto()
                {
                    Id = comment.Id,
                    User = comment.User,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
        }
        
        private PostDto ToShortenedDto(Post post)
        {
            return new()
            {
                Id = post.Id,
                User = post.User,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                Comments = post.Comments
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(2) // get last 2 comments
                    .Select(comment => new CommentDto()
                {
                    Id = comment.Id,
                    User = comment.User,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
        }
    }
}