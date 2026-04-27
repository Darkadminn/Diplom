using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeApplication
{
    public class HospitalProcedure
    {
        public int id {  get; set; }
        public DateTime date { get; set; }
        public int medicalServiceId { get; set; }
        public string medicalService { get; set; }
        public int count { get; set; }
        public string description { get; set; }
        public int countСompleted { get; set; }
        public Brush brushColor => count == countСompleted ? Brushes.Green : Brushes.Red;
    }
}
