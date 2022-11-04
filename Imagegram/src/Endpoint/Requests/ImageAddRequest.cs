using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Requests
{
    public class ImageAddRequest
    {
        [Required]
        public string PostId { get; init; }
        [Required]
        public IFormFile Image { get; init; }
    }
}