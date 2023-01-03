using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Requests
{
    public class UserSearchRequest
    {
        public string? searchQuery { get; set; } = string.Empty;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 3;
    }
}
