using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Doctor
    {
        public int Doctor_Id { get; set; }
        public string Full_Name { get; set; }
        public string Specialization { get; set; }
        public string Contact_Number { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
