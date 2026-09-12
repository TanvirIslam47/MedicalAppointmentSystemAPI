using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class AppointmentListDto
    {
        public int? AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string? PatientEmail { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string VisitType { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }

        public List<PrescriptionDto> Prescriptions { get; set; }
    }
}
