using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class AppointmentSaveDto
    {
        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string ContactNumber { get; set; }
        public required string Address { get; set; }
        [Required, EmailAddress]
        public required string PatientEmail { get; set; }
        public int DepartmentId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string Notes { get; set; }
    }

}
