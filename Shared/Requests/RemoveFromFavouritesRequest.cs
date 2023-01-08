using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class RemoveFromFavouritesRequest
    {
        [Required]
        public int ArticleId { get; set; }
    }
}
