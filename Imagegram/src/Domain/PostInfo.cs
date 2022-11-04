using System;

namespace Domain
{
    public class PostInfo
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string User { get; init; }
        public string Caption { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}