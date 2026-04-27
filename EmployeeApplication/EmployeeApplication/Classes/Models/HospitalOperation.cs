using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class HospitalOperation
    {
        public int id {  get; set; }
        public int medicalServiceId { get; set; }
        public string medicalService { get; set; }
        public DateTime date { get; set; }
        public bool result { get; set; }
    }
}
