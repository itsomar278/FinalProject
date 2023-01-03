using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO_s.ResponseDto_s
{
    public class CommentResponseDto
    {
        public string CommentContent { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
