using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class MedicalService
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string info => $"{code} {name}";
        public bool isAnalisis => code.Contains("A09") ? true : false;
        public bool isResearche => (code.Contains("A01") || code.Contains("A02") || code.Contains("A03")
                                                || code.Contains("A04") || code.Contains("A05") || code.Contains("A06")
                                                || code.Contains("A07") || code.Contains("A08") || code.Contains("A09")
                                                || code.Contains("A10") || code.Contains("A11") || code.Contains("A12")
                                                || code.Contains("A13")) ? true : false;
        public bool isOperation => code.Contains("A16") ? true : false;
        public bool isProcedure => !isResearche && !isOperation;
    }
}
