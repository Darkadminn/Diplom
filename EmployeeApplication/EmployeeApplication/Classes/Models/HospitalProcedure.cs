using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class HospitalProcedure
    {
        public int id {  get; set; }
        public DateTime date { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string description { get; set; }
        public int countСompleted { get; set; }
    }
}
