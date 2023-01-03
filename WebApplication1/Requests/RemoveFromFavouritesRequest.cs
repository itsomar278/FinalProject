using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class RemoveFromFavouritesRequest
    {
        [Required]
        public int ArticleId { get; set; }
    }
}
