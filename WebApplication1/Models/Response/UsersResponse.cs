using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Response
{
    public class UsersResponse
    {
        [Required, MinLength(6), MaxLength(30)]
        public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string UserEmail { get; set; } = string.Empty;
    }
}
