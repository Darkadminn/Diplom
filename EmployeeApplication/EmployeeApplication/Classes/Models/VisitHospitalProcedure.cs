using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeApplication
{
    public class VisitHospitalProcedure
    {
        public int id { get; set; }
        public int medicalServiceId { get; set; }
        public string medicalService { get; set; }
        public string comment { get; set; }
        public int count { get; set; }
        public DateTime date { get; set; }
        public string patient { get; set; }
        public string employee { get; set; }
        public int employeeId { get; set; }
        public string result { get; set; }
        public string post { get; set; }
        public string type { get; set; }
        public string code { get; set; }
        public int countСompleted { get; set; }
        public Brush brushColor => count == countСompleted ? Brushes.Green : Brushes.Red;
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
