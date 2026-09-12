using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Patient
    {
        public int Patient_Id { get; set; }
        public string Full_Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Contact_Number { get; set; }
        public string Address { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
