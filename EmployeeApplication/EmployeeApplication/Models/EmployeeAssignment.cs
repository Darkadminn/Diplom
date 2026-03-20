using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class EmployeeAssignment
    {
        public int id { get; set; }
        public int employeeId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string type { get; set; }
        public int postId { get; set; }
        public string post { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public int wingId { get; set; }
        public string wing {  get; set; }
        public int cabinetId { get; set; }
        public string cabinet { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
    }
}
