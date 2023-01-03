using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string UserEmail { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
