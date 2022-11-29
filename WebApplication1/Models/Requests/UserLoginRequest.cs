using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class UserLoginRequest
    {
        [Required, EmailAddress]
        public string UserEmail { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
