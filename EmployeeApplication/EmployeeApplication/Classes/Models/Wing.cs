using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class Wing
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string home { get; set; }
        public int medicalInstitutionId { get; set; }
        public string medicalInstitution { get; set; }
        public string type { get; set; }
        public string typeIndividual { get; set; }
        public string info => $"{name} {typeIndividual}";
    }
}
