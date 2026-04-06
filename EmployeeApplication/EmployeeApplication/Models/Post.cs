using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class Post
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string info => $"{name} {type}";
    }
}
