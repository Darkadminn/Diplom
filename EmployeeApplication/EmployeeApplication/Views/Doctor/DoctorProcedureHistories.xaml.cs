using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeeApplication
{
    /// <summary>
    /// Логика взаимодействия для DoctorProcedureHistories.xaml
    /// </summary>
    public partial class DoctorProcedureHistories : Window
    {
        DB dB = new DB();
        List<VisitProcedureHistory> visitProcedureHistories = new List<VisitProcedureHistory>();
        public DoctorProcedureHistories(VisitProcedure visitProcedure)
        {
            InitializeComponent();

            visitProcedureHistories = dB.GetVisitProcedureHistories(visitProcedure.id);
            DataGridProcedureHistories.ItemsSource = visitProcedureHistories;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
