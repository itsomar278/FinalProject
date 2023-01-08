using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class ArticlePostRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Title must be atleast 8 characters and less than 200 characters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
