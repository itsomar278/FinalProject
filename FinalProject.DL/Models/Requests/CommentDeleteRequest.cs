using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class CommentDeleteRequest
    {
        [Required]
        public int commentId;
    }
}
