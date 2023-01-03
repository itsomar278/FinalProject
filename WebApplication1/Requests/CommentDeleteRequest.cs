using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class CommentDeleteRequest
    {
        [Required]
        public int commentId;
    }
}
