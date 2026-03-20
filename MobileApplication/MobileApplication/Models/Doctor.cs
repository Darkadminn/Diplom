using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class Doctor
    {
        public int id { get; set; }
        public int assignmentId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string fio => $"{lastName} {firstName} {middleName}".Trim();
        public int postId { get; set; }
        public string postName { get; set; }
        public TimeSpan timeInterval { get; set; }
        public int wingId { get; set; }
        public string wingName { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string home { get; set; }


    }
}
