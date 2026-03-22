using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class Individual
    {
        public int id { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public DateTime birthday { get; set; }
        public string phone { get; set; }
        public string snils { get; set; }
        public string passportSeries { get; set; }
        public string passportNumber { get; set; }
        public string passportIssuedBy { get; set; }
        public DateTime passportIssuedDate { get; set; }
        public string gender { get; set; }
        public string insurancePolicy { get; set; }
        public string insuranceCompany { get; set; }
        public string birthCertificate { get; set; }
        public string info => $"{lastName} {firstName} {middleName} {snils}".Trim();
    }
}
