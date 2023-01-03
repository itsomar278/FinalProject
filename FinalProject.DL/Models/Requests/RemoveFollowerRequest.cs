using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Requests
{
    public class RemoveFollowerRequest
    {
        public int FollowerToRemoveId { get; set; }
    }
}
