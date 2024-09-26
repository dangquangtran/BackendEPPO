using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Services
{
    public class ServicesDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public double? ServicePrice { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string Code { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
    }
}
