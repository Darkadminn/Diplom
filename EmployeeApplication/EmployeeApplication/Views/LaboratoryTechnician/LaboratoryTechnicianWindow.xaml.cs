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
    /// Логика взаимодействия для LaboratoryTechnicianWindow.xaml
    /// </summary>
    public partial class LaboratoryTechnicianWindow : Window
    {
        DB dB = new DB();
        List<VisitHospitalProcedure> procedures = new List<VisitHospitalProcedure>();
        List<VisitHospitalProcedure> filterProcedures = new List<VisitHospitalProcedure>();
        public LaboratoryTechnicianWindow()
        {
            InitializeComponent();

            TextBlockUser.Text = UserAuthorization.fio;
            TextBlockPostUser.Text = UserAuthorization.postName;

            StackPanelProcedures.Visibility = Visibility.Collapsed;

            SearchProcedure.TextChanged += (a, args) =>
            {
                FilterProcedures();
            };

            FilterProcedure.SelectionChanged += (a, args) =>
            {
                FilterProcedures();
            };
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (this.Height - 180 >= 30) DataGridProcedures.Height = this.Height - 180;
            else DataGridProcedures.Height = 30;


            if (this.Height - 170 >= 30) StackExit.Height = this.Height - 170;
            else StackExit.Height = 30;
        }

        private void ButtonClickExit(object sender, RoutedEventArgs e)
        {
            UserAuthorization.id = -1;
            UserAuthorization.role = "";
            UserAuthorization.fio = "";
            UserAuthorization.employeeId = -1;
            UserAuthorization.isOperation = false;
            UserAuthorization.wingType = "";
            UserAuthorization.postName = "";
            UserAuthorization.wingId = -1;

            var window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void ButtonClickProcedures(object sender, RoutedEventArgs e)
        {
            TextBlockHeader.Text = "Анализы";

            StackPanelProcedures.Visibility = Visibility.Visible;

            LoadProcedures();
        }

        private void LoadProcedures()
        {
            procedures = dB.GetVisitHospitalProcedures().Where(pr => pr.isAnalisis == true).ToList();

            FilterProcedures();
        }

        private void FilterProcedures()
        {
            filterProcedures = procedures;

            if (!string.IsNullOrEmpty(SearchProcedure.Text))
            {
                filterProcedures = filterProcedures.Where(p => p.patient.ToLower().Contains(SearchProcedure.Text.ToLower())
                                                            || p.medicalService.ToLower().Contains(SearchProcedure.Text.ToLower())
                                                            || p.employee.ToLower().Contains(SearchProcedure.Text.ToLower())
                                                            || p.type.ToLower().Contains(SearchProcedure.Text.ToLower())).ToList();
            }

            if (FilterProcedure.SelectedIndex > 0)
            {
                if (FilterProcedure.SelectedIndex == 1) filterProcedures = filterProcedures.Where(p => p.count != p.countСompleted).ToList();
                else if (FilterProcedure.SelectedIndex == 2) filterProcedures = filterProcedures.Where(p => p.count == p.countСompleted).ToList();
            }

            DataGridProcedures.ItemsSource = filterProcedures;
        }

        private void DataGridProcedures_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridProcedures.SelectedItem != null)
            {
                var procedure = DataGridProcedures.SelectedItem as VisitHospitalProcedure;

                var window = new LaboratoryTechnicianProcedureHistories(procedure);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadProcedures();
                }
            }
        }
    }
}
