using System.ComponentModel.DataAnnotations;

namespace TaskMgmAPI.DTOfile
{
    public class LoginDto
    {
        //[Required]
        //[EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }
    }
}
