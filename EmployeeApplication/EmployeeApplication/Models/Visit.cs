using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class Visit
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string diagnosis { get; set; }
        public string objective { get; set; }
        public string subjective { get; set; }
        public string recommendation { get; set; }
        public int employeeAssignmentId { get; set; }
        public int patientId { get; set; }
        public string patient {  get; set; }
        public string status { get; set; }
    }
}
