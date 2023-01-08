using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests
{
    public class ArticlesSearchRequest
    {
        public string? title { get; set; } = string.Empty;
        public string? searchQuery { get; set; } = string.Empty;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 2;
    }
}
