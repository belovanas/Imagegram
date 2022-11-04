using System.ComponentModel.DataAnnotations;

namespace Imagegram.Requests
{
    public class UserCreateRequest
    {
        [Required] public string Login { get; set; }
    }
}