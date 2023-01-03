using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class CommentPostRequest
    {
        [Required]
        public string CommentContent { get; set; } = string.Empty;
    }
}
