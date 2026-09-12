using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class AppointmentDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string VisitType { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }

        public List<PrescriptionDto> Prescriptions { get; set; }
    }
}
