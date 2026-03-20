using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class Wing
    {
        public int id { get; set; }
        public string name { get; set; }
        public string addressCity { get; set; }
        public string addressStreet { get; set; }
        public string addressHome { get; set; }
        public string type { get; set; }
        public string typeIndividual { get; set; }
        public string info => $"{name}\tг. {addressCity} ул. {addressStreet} д. {addressHome}";
    }
}
