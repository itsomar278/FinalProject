using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class RemoveFromFavouritesRequest
    {
        [Required]
        public int ArticleId { get; set; }
    }
}
