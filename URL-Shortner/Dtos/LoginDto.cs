using System.ComponentModel.DataAnnotations;

namespace URL_Shortner.Dtos
{
    public class LoginDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
