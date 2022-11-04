using System;

namespace Domain
{
    public class Comment
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string PostId { get; init; }
        public string User { get; init; }
        public string Content { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}