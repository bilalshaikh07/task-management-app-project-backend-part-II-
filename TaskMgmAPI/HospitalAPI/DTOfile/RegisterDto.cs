using System.ComponentModel.DataAnnotations;

namespace TaskMgmAPI.DTOfile
{
    public class RegisterDto
    {

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Organization { get; set; }

        public string? Designation { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
    }
}
