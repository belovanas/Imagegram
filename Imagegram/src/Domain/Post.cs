using System;
using System.Collections.Generic;

namespace Domain
{
    public class Post
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string Caption { get; init; }
        public string ImageLink { get; set; }
        public string User { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public List<Comment> Comments { get; set; }
    }
}