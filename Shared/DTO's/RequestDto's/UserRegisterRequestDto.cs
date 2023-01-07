using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO_s.RequestDto_s
{
    public class UserRegisterRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
