using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class AddToFavouritesRequest
    {
        [Required]
        public int ArticleId { get; set; }
    }
}
