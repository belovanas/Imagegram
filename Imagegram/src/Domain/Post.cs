using System;
using System.Collections.Generic;
using System.IO;

namespace Domain
{
    public class Post
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string User { get; init; }
        public string Caption { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public List<Comment> Comments { get; set; }
    }
}