using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Medicine
    {
        public int Medicine_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
