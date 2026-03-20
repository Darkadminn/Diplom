using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class Children
    {
        public int id {  get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public DateTime birthday { get; set; }
        public string gender { get; set; }
        public int wingId { get; set; }
        public string? insurancePolicy { get; set; }
        public string? insuranceCompany { get; set; }
        public string? snils { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
    }
}
