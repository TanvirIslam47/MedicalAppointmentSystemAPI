using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class RegisterUserDto
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
