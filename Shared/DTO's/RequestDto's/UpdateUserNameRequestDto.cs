using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO_s.RequestDto_s
{
    public class UpdateUserNameRequestDto
    {
        public string NewUserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string userPassword { get; set; } = string.Empty;

    }
}
