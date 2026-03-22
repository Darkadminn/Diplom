using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class Children
    {
        public int id { get; set; }
        public int parentId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public DateTime birthday { get; set; }
        public string birthCertificate { get; set; }
        public string info => $"{lastName} {firstName} {middleName} {birthCertificate}".Trim();
    }
}
