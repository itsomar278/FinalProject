using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class ArticlePostRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
