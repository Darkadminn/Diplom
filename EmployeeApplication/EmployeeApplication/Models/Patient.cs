using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class Patient
    {
        public int id {  get; set; }
        public int individualId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public int wingId { get; set; }
        public string wing {  get; set; }
        public DateTime birthday { get; set; }
        public string phone {  get; set; }
        public string snils { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
    }
}
