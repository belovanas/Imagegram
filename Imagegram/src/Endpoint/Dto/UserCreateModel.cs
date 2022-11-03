using System.ComponentModel.DataAnnotations;

namespace Imagegram.Dto
{
    public class UserCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
}