using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Imagegram.Dto
{
    public class PostDto
    {
        [Required]
        public string Id { get; init; }
        [Required]
        public string User { get; init; }
        [Required]
        public string Caption { get; init; }
        [Required]
        public DateTime CreatedAt { get; init; }
        public List<CommentDto> Comments { get; init; }
    }
}