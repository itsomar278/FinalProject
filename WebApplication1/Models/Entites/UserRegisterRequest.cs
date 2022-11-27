
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entites
{
	public class UserRegisterRequest
	{
        [Required , MinLength(6) , MaxLength(30)]
        public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress]
		public string UserEmail { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string PasswordConfirm { get; set; } = string.Empty;

    }
}
