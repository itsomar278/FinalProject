using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class CommentDeleteRequest
    {
        [Required]
        public int commentId;
    }
}
