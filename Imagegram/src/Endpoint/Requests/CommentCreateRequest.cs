using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests
{
    public class CommentCreateRequest
    {
        [Required] public string Content { get; set; }
    }
}