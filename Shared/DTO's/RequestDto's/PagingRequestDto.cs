using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO_s.RequestDto_s
{
    public class PagingRequestDto
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 2;
    }
}
