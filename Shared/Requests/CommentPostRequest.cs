using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class CommentPostRequest
    {
        [Required]
        public string CommentContent { get; set; } = string.Empty;
    }
}
