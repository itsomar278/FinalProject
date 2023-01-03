using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class UpdateUserNameRequest
    {
        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "field must be atleast 6 characters and less than 30 characters")]
        public string NewUserName { get; set;} = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string UserEmail { get; set;} = string.Empty;
        [Required]
        public string userPassword { get; set; } = string.Empty;
    }
}
