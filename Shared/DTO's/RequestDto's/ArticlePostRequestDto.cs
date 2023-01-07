using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO_s.RequestDto_s
{
    public class ArticlePostRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
