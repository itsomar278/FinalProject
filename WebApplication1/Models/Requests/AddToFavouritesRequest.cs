using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class AddToFavouritesRequest
    {
        [Required]
        public int ArticleId { get; set; }
    }
}
