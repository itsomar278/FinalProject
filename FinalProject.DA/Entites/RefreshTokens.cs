using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAcess.Entites
{
    public class RefreshTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        [Required]
        public DateTime Expires { get; set; }
        public int UserId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Users User { get; set; }
    }
}
