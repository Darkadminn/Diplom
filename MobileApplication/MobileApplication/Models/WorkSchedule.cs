using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class WorkSchedule
    {
        public int id { get; set; }
        public TimeSpan timeStart { get; set; }
        public TimeSpan timeEnd { get; set; }
        public string[] weekDays { get; set; }
        public string cabinetName { get; set; }
    }
}
