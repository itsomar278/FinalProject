using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        [Required]
        public string CommentContent { get; set; } = string.Empty;
        [Required]
        public int UserId { get; set; }
        public Users User { get; set; }
        [Required]
        public int ArticleId { get; set; }
        public Articles Article { get; set; }
    }
}
