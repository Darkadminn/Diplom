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
    /// Логика взаимодействия для DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        DB dB = new DB();
        List<Visit> visits = new List<Visit>();
        List<VisitProcedure> procedures = new List<VisitProcedure>();
        List<Treatment> treatments = new List<Treatment>();
        List<Visit> filterVisits = new List<Visit>();
        List<Visit> currentVisits = new List<Visit>();
        List<VisitProcedure> filterProcedures = new List<VisitProcedure>();
        List<Treatment> filterTreatments = new List<Treatment>();
        List<Treatment> currentTreatments = new List<Treatment>();
        public DoctorWindow()
        {
            InitializeComponent();

            TextBlockUser.Text = UserAuthorization.fio;
            TextBlockPostUser.Text = UserAuthorization.postName;

            StackPanelVisits.Visibility = Visibility.Collapsed;
            StackPanelProcedures.Visibility = Visibility.Collapsed;
            StackPanelTreatments.Visibility = Visibility.Collapsed;

            SearchVisit.TextChanged += (a, args) =>
            {
                FilterVisits();
            };

            VisitStatuses.SelectionChanged += (a, args) =>
            {
                FilterVisits();
            };

            SearchProcedure.TextChanged += (a, args) =>
            {
                FilterProcedures();
            };

            FilterProcedure.SelectionChanged += (a, args) =>
            {
                FilterProcedures();
            };

            SearchTreatmentPatient.TextChanged += (a, args) =>
            {
                FilterTreatments();
            };

            FilterTreatment.SelectionChanged += (a, args) =>
            {
                FilterTreatments();
            };
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if ((this.Height - 200) / 2 >= 30)
            {
                DataGridCurrentVisits.Height = (this.Height - 200) / 2;
                DataGridVisits.Height = (this.Height - 200) / 2;
                DataGridCurrentTreatments.Height = (this.Height - 200) / 2;
                DataGridTreatments.Height = (this.Height - 200) / 2;
            }
            else
            {
                DataGridCurrentVisits.Height = 30;
                DataGridVisits.Height = 30;
                DataGridCurrentTreatments.Height = 30;
                DataGridTreatments.Height = 30;
            }

            if (this.Height - 180 >= 30) DataGridProcedures.Height = this.Height - 180;
            else DataGridProcedures.Height = 30;


            if (this.Height - 230 >= 30) StackExit.Height = this.Height - 230;
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

            var window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void ButtonClickVisits(object sender, RoutedEventArgs e)
        {
            TextBlockHeader.Text = "Приемы";

            StackPanelVisits.Visibility = Visibility.Visible;
            StackPanelProcedures.Visibility = Visibility.Collapsed;
            StackPanelTreatments.Visibility = Visibility.Collapsed;

            LoadVisits();
        }

        private void ButtonClickProcedures(object sender, RoutedEventArgs e)
        {
            TextBlockHeader.Text = "Процедуры";

            StackPanelVisits.Visibility = Visibility.Collapsed;
            StackPanelProcedures.Visibility = Visibility.Visible;
            StackPanelTreatments.Visibility = Visibility.Collapsed;

            LoadProcedures();
        }

        private void ButtonClickTreatments(object sender, RoutedEventArgs e)
        {
            TextBlockHeader.Text = "Курсы лечения";

            StackPanelVisits.Visibility = Visibility.Collapsed;
            StackPanelProcedures.Visibility = Visibility.Collapsed;
            StackPanelTreatments.Visibility = Visibility.Visible;

            LoadTreatments();
        }

        private void LoadVisits()
        {
            visits = dB.GetVisits();

            FilterVisits();
        }

        private void FilterVisits()
        {
            filterVisits = visits;

            if (!string.IsNullOrEmpty(SearchVisit.Text))
            {
                filterVisits = filterVisits.Where(v => v.patient.ToLower().Contains(SearchVisit.Text.ToLower())).ToList();
            }

            if(VisitStatuses.SelectedIndex > 0)
            {
                if(VisitStatuses.SelectedIndex == 1) filterVisits = filterVisits.Where(v => v.status == "Запланирован").ToList();
                else if (VisitStatuses.SelectedIndex == 2) filterVisits = filterVisits.Where(v => v.status == "Завершен").ToList();
                else if (VisitStatuses.SelectedIndex == 3) filterVisits = filterVisits.Where(v => v.status == "Перенесен").ToList();
                else if (VisitStatuses.SelectedIndex == 4) filterVisits = filterVisits.Where(v => v.status == "Отменен").ToList();
            }

            currentVisits = visits.Where(v => (v.status == "Запланирован" || v.status == "Перенесен") && v.date.Date == DateTime.Now.Date).ToList();

            DataGridVisits.ItemsSource = filterVisits;
            DataGridCurrentVisits.ItemsSource = currentVisits;
        }

        private void DataGridCurrentVisits_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataGridCurrentVisits.SelectedItem != null)
            {
                var visit = DataGridCurrentVisits.SelectedItem as Visit;

                var window = new DoctorAddUpdateVisit(visit, false);
                window.ShowDialog();

                if(window.DialogResult == true)
                {
                    LoadVisits();
                }
            }
        }

        private void DataGridVisits_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridVisits.SelectedItem != null)
            {
                var visit = DataGridVisits.SelectedItem as Visit;

                var window = new DoctorAddUpdateVisit(visit, true);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadVisits();
                }
            }
        }

        private void LoadProcedures()
        {
            procedures = dB.GetVisitProcedures();

            FilterProcedures();
        }

        private void FilterProcedures()
        {
            filterProcedures = procedures;

            if (!string.IsNullOrEmpty(SearchProcedure.Text))
            {
                filterProcedures = filterProcedures.Where(p => p.patient.ToLower().Contains(SearchProcedure.Text.ToLower())
                                                            || p.medicalProcedure.ToLower().Contains(SearchProcedure.Text.ToLower())).ToList();
            }

            if(FilterProcedure.SelectedIndex > 0)
            {
                if(FilterProcedure.SelectedIndex == 1) filterProcedures = filterProcedures.Where(p => p.count != p.countСompleted).ToList();
                else if (FilterProcedure.SelectedIndex == 2) filterProcedures = filterProcedures.Where(p => p.count == p.countСompleted).ToList();
            }

            DataGridProcedures.ItemsSource = filterProcedures;
        }

        private void DataGridProcedures_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataGridProcedures.SelectedItem != null)
            {
                var procedure = DataGridProcedures.SelectedItem as VisitProcedure;

                var window = new DoctorProcedureHistories(procedure);
                window.ShowDialog();
            }
        }

        private void LoadTreatments()
        {
            treatments = dB.GetTreatments();

            FilterTreatments();
        }

        private void FilterTreatments()
        {
            filterTreatments = treatments;

            if (!string.IsNullOrEmpty(SearchTreatmentPatient.Text))
            {
                filterTreatments = filterTreatments.Where(t => t.patient.ToLower().Contains(SearchVisit.Text.ToLower())).ToList();
            }

            if (FilterTreatment.SelectedIndex > 0)
            {
                if (FilterTreatment.SelectedIndex == 1) filterTreatments = filterTreatments.Where(t => t.dateEnd != null).ToList();
                else if (FilterTreatment.SelectedIndex == 2) filterTreatments = filterTreatments.Where(t => t.dateEnd == null).ToList();
            }

            currentTreatments = treatments.Where(t => t.dateEnd != null).ToList();

            DataGridTreatments.ItemsSource = filterTreatments;
            DataGridCurrentTreatments.ItemsSource = currentTreatments;
        }

        private void DataGridCurrentTreatments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridCurrentTreatments.SelectedItem != null)
            {
                var treatment = DataGridCurrentTreatments.SelectedItem as Treatment;

                var window = new DoctorAddUpdateTreatment(treatment, false);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadTreatments();
                }
            }
        }

        private void DataGridTreatments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridTreatments.SelectedItem != null)
            {
                var treatment = DataGridTreatments.SelectedItem as Treatment;

                var window = new DoctorAddUpdateTreatment(treatment, true);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadTreatments();
                }
            }
        }

        
    }
}
