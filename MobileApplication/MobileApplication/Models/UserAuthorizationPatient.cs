using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class UserAuthorizationPatient
    {
        public int id { get; set; }
        public string fio { get; set; }
        public string gender { get; set; }
        public int polyclinicId { get; set; }
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
    }
}
