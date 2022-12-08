using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entites
{
    public class Favorite
    {
        public int UserId { get; set; }
        public Users User { get; set; }
        public int ArticleId { get; set; }
        public Articles Article { get; set; }
    }
}
