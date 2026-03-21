using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    public class User
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string assignment { get; set; }
        public string role { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int wingId { get; set; }
        public string post { get; set; }
        public string wing { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
        public string info => $"{fio} {post} {assignment} ";
        public string passwordSecurity => password.Replace(password, "************");
    }
}
