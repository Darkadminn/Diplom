using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeApplication
{
    public class VisitProcedure
    {
        public int id {  get; set; }
        public int medicalProcedureId { get; set; }
        public string medicalProcedure { get; set; }
        public string comment { get; set; }
        public int count { get; set; }
        public int visitId { get; set; }
        public string patient {  get; set; }
        public string employee { get; set; }
        public string post {  get; set; }
        public int countСompleted { get; set; }
        public Brush brushColor => count == countСompleted ? Brushes.Green : Brushes.Red;
    }
}
