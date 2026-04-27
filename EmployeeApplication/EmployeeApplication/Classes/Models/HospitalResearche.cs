using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeApplication
{
    internal class HospitalResearche
    {
        public int id {  get; set; }
        public DateTime date { get; set; }
        public int medicalServiceId { get; set; }
        public string medicalService { get; set; }
        public string result { get; set; }
        public bool isCompleted { get; set; }
        public Brush brushColor => isCompleted ? Brushes.Green : Brushes.Red;

    }
}
