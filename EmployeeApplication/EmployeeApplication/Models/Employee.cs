using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class Employee
    {
        public int id { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public DateTime birthday { get; set; }
        public int postId { get; set; }
        public string post { get; set; }
        public string postType { get; set; }
        public DateTime dateAdmission { get; set; }
        public DateTime? dateDismissal { get; set; }
        public int individualId { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
        public string info => $"{fio}  {post}";
    }
}
