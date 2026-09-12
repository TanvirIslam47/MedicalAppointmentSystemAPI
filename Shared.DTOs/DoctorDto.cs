using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }            
        public required string FullName { get; set; }
        public required string Specialization { get; set; }
        public required string ContactNumber { get; set; }
    }

}
