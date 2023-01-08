using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "User name is required.")]
        [RegularExpression("^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "Username must contain only alphanumeric characters.")]
        [MinLength(6, ErrorMessage = "User name must be at least 6 characters long.")]
        [MaxLength(30, ErrorMessage = "User name must be no more than 30 characters long.")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string UserEmail { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = " passwords dont match !")]
        public string PasswordConfirm { get; set; } = string.Empty;

    }
}
