using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entites
{
    public class Users
    {
        public Users()
        {
            PublishedArticles = new List<Articles>();
            FavoriteArticles = new List<Articles>();
            Followers = new List<Follow>();
            Following = new List<Follow>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "field must be atleast 6 characters and less than 30 characters")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string UserEmail { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public int? RefreshTokenId { get; set; }
        public RefreshTokens? RefreshToken { get; set; }
        public List<Articles> PublishedArticles { get; set; }
        public List<Articles> FavoriteArticles { get; set; }
        public List<Follow> Followers { get; set; }
        public List<Follow> Following { get; set; }
    }
}
