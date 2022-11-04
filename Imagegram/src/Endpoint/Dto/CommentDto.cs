using System;
using System.ComponentModel.DataAnnotations;

namespace Imagegram.Dto
{
    public class CommentDto
    {
        [Required]
        public string Id { get; init; }
        [Required]
        public string User { get; init; }
        [Required]
        public string Content { get; init; }
        [Required]
        public DateTime CreatedAt { get; init; }
    }
}