using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Articles
    {
        public Articles()
        {
            Comments = new List<Comments>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Title must be atleast 8 characters and less than 200 characters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public Users User { get; set; }
        public List<Comments> Comments { get; set; }

    }
}
