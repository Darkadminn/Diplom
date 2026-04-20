using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeApplication
{
    public class Treatment
    {
        public int id { get; set; }
        public DateTime dateVisit { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public int visitId { get; set; }
        public string patient {  get; set; }
        public string patientGender { get; set; }
        public int patientId { get; set; }
        public string diagnosis { get; set; }

        public Brush brushColor => dateEnd == null ? Brushes.Green : Brushes.Red;
    }
}
