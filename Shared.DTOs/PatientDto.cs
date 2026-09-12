using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class PatientDto
    {
        public int PatientId { get; set; }             
        public string FullName { get; set; }           
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }            
        public string ContactNumber { get; set; }       
        public string Address { get; set; }            
    }

}
