using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Requests
{
    public class PagingRequest
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 2;
    }
}
