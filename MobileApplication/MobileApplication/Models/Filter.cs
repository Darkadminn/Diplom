using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class Filter
    {
        public string name { get; set; }
        public string type { get; set; }
        public string info => $"{name} \n {type}";
    }
}
