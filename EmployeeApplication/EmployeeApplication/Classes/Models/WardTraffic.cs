using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class WardTraffic
    {
        public int id {  get; set; }
        public int wardId { get; set; }
        public string ward {  get; set; }
        public int treatmentId { get; set; }
        public string department { get; set; }
        public int departmentId { get; set; }
        public DateTime dateArrival { get; set; }
        public DateTime? dateDeparture { get; set; }
    }
}
