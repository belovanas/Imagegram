using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Requests
{
    public class PostCreateRequest
    {
        [Required]
        public string Caption { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}