using System;
using System.Collections.Generic;
using System.IO;

namespace Domain
{
    public class Post
    {
        public PostInfo Info { get; init; }
        public List<Comment> Comments { get; init; }
    }
}