using System.Linq;
using Domain;
using Imagegram.Dto;

namespace Imagegram.Mapping
{
    internal static class PostMapping
    {
        internal static PostDto ToDto(this Post post)
        {
            return new()
            {
                Id = post.Id,
                User = post.User,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                Comments = post.Comments.Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    User = comment.User,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
        }

        internal static PostDto ToShortenedDto(this Post post)
        {
            return new()
            {
                Id = post.Id,
                User = post.User,
                Caption = post.Caption,
                ImageLink = post.ImageLink,
                CreatedAt = post.CreatedAt,
                Comments = post.Comments
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(2) // get last 2 comments
                    .Select(comment => new CommentDto
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