using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class PrescriptionDetail
    {
        public int Prescription_Id { get; set; }
        public int Appointment_Id { get; set; }
        public int Medicine_Id { get; set; }
        public string Dosage { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public string Notes { get; set; }

        public Appointment Appointment { get; set; }
        public Medicine Medicine { get; set; }
    }
}
