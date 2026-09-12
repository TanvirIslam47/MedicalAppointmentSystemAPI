using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Appointment
    {
        public int Appointment_Id { get; set; }
        public int Patient_Id { get; set; }
        public int Doctor_Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Visit_Type { get; set; } 
        public string Diagnosis { get; set; }
        public string Notes { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
