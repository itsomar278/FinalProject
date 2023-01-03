using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class CommentPostRequest
    {
        [Required]
        public string CommentContent { get; set; } = string.Empty;
    }
}
