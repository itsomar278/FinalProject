using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class UserPasswordUpdateRequest
    {
        [Required, EmailAddress]
        public string UserEmail { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string OldPassword { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string NewPassword { get; set; }
        [Required, MinLength(8) , Compare("NewPassword")]
        public string ConfirmPassword { get; set;}
    }
}
