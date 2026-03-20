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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        DB dB = new DB();
        List<Patient> patients = new List<Patient>();
        List<Patient> filterPacients = new List<Patient>();
        List<Employee> employees = new List<Employee>();
        List<Employee> filterEmployees = new List<Employee>();
        List<Wing> wings = new List<Wing>();
        List<Wing> wingsAll = new List<Wing>();
        public AdminWindow()
        {
            InitializeComponent();
            StackPanelExit.Height = this.Height - 290;
            TextBlockUser.Text = UserAuthorization.fio;
            TextBlockPostUser.Text = UserAuthorization.postName;

            StackPanelPatients.Visibility = Visibility.Collapsed;
            StackPanelEmployees.Visibility = Visibility.Collapsed;
            StackPanelEmployeeAssignments.Visibility = Visibility.Collapsed;
            StackPanelUsers.Visibility = Visibility.Collapsed;
            StackPanelReports.Visibility = Visibility.Collapsed;

            wings = dB.GetPoliclinicWings();
            wings.Insert(0, new Wing
            {
                id = 0,
                name = "Все"
            });

            wingsAll = dB.GetWings();
            wingsAll.Insert(0, new Wing
            {
                id = 0,
                name = "Все"
            });

            FilterPatient.ItemsSource = wings;

            SearchPatient.TextChanged += (a, args) =>
            {
                FilterPatients();
            };

            FilterPatient.SelectionChanged += (a, args) =>
            {
                FilterPatients();
            };


            SearchEmployee.TextChanged += (a, args) =>
            {
                FilterEmployees();
            };

            FilterEmployee.SelectionChanged += (a, args) =>
            {
                FilterEmployees();
            };
        }

        /// <summary>
        /// Изменение размера окна
        /// </summary>
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Height >= 290)
            {
                StackPanelExit.Height = this.Height - 290;
            }
            else
            {
                StackPanelExit.Height = 30;
            }

            ScrollViewerWindow.Height = this.Height - 76;

            if (this.Width > 510)
            {
                double margin = (this.Width - 550) / 2;
                StackPanelSearchPatient.Margin = new Thickness(0, 0, margin, 0);
                StackPanelFilterPatient.Margin = new Thickness(0, 0, margin, 0);
                DataGridPatients.Width = this.Width - 155;

                StackPanelSearchEmployee.Margin = new Thickness(0, 0, margin, 0);
                StackPanelFilterEmployee.Margin = new Thickness(0, 0, margin, 0);
                DataGridEmployees.Width = this.Width - 155;
            }

            if (ScrollViewerWindow.Height > 100)
            {
                DataGridPatients.Height = ScrollViewerWindow.Height - 58;
                DataGridEmployees.Height = ScrollViewerWindow.Height - 58;
            }
        }

        /// <summary>
        /// Переключение разделов
        /// </summary>
        private void ButtonClickPatients(object sender, RoutedEventArgs e)
        {
            StackPanelPatients.Visibility = Visibility.Visible;
            StackPanelEmployees.Visibility = Visibility.Collapsed;
            StackPanelEmployeeAssignments.Visibility = Visibility.Collapsed;
            StackPanelUsers.Visibility = Visibility.Collapsed;
            StackPanelReports.Visibility = Visibility.Collapsed;

            TextBlockHeader.Text = "Пациенты";
            TextBlockHeader.FontWeight = FontWeights.Bold;

            LoadPatients();          
        }

        private void ButtonClickEmployees(object sender, RoutedEventArgs e)
        {
            StackPanelEmployees.Visibility = Visibility.Visible;
            StackPanelPatients.Visibility = Visibility.Collapsed;
            StackPanelEmployeeAssignments.Visibility = Visibility.Collapsed;
            StackPanelUsers.Visibility = Visibility.Collapsed;
            StackPanelReports.Visibility = Visibility.Collapsed;

            TextBlockHeader.Text = "Сотрудники";
            TextBlockHeader.FontWeight = FontWeights.Bold;

            LoadEmployees();
        }

        private void ButtonClickEmployeeAssignments(object sender, RoutedEventArgs e)
        {
            StackPanelEmployeeAssignments.Visibility = Visibility.Visible;
            StackPanelEmployees.Visibility = Visibility.Collapsed;
            StackPanelPatients.Visibility = Visibility.Collapsed;
            StackPanelUsers.Visibility = Visibility.Collapsed;
            StackPanelReports.Visibility = Visibility.Collapsed;

            TextBlockHeader.Text = "Назначения сотрудников";
            TextBlockHeader.FontWeight = FontWeights.Bold;
        }

        private void ButtonClickUsers(object sender, RoutedEventArgs e)
        {
            StackPanelUsers.Visibility = Visibility.Visible;
            StackPanelPatients.Visibility = Visibility.Collapsed;
            StackPanelEmployeeAssignments.Visibility = Visibility.Collapsed;
            StackPanelEmployees.Visibility = Visibility.Collapsed;
            StackPanelReports.Visibility = Visibility.Collapsed;

            TextBlockHeader.Text = "Пользователи";
            TextBlockHeader.FontWeight = FontWeights.Bold;
        }

        private void ButtonClickReports(object sender, RoutedEventArgs e)
        {
            StackPanelReports.Visibility = Visibility.Visible;
            StackPanelUsers.Visibility = Visibility.Collapsed;
            StackPanelEmployeeAssignments.Visibility = Visibility.Collapsed;
            StackPanelPatients.Visibility = Visibility.Collapsed;
            StackPanelEmployees.Visibility = Visibility.Collapsed;

            TextBlockHeader.Text = "Отчеты";
            TextBlockHeader.FontWeight = FontWeights.Bold;
        }

        /// <summary>
        /// Выход из аккаунта
        /// </summary>
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

        private void ButtonClickAddPatient(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdatePatient();
            window.ShowDialog();
        }

        private void DataGridPatientsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataGridPatients.SelectedItem != null)
            {
                var patient = DataGridPatients.SelectedItem as Patient;

                var window = new AdminAddUpdatePatient(patient);
                window.ShowDialog();
            }
        }

        private void LoadPatients()
        {
            patients = dB.GetPatients();

            DataGridPatients.ItemsSource = patients;

            FilterPatients();
        }

        private void FilterPatients()
        {
            filterPacients = patients;

            if (!string.IsNullOrEmpty(SearchPatient.Text))
            {
                filterPacients = filterPacients.Where(p => p.lastName.ToLower().Contains(SearchPatient.Text.ToLower()) || p.firstName.ToLower().Contains(SearchPatient.Text.ToLower())
                                                || p.middleName.ToLower().Contains(SearchPatient.Text.ToLower()) || p.wing.ToLower().Contains(SearchPatient.Text.ToLower())
                                                || p.phone.ToLower().Contains(SearchPatient.Text.ToLower()) || p.snils.ToLower().Contains(SearchPatient.Text.ToLower())).ToList();
            }

            if(FilterPatient.SelectedItem != null && FilterPatient.SelectedIndex != 0)
            {
                var filter = FilterPatient.SelectedItem as Wing;

                filterPacients = filterPacients.Where(p => p.wingId == filter.id).ToList();
            }

            DataGridPatients.ItemsSource = filterPacients;
            DataGridPatients.Items.Refresh();

            
        }

        private void ButtonClickDeletePatient(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickAddEmployee(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdateEmployee();
            window.ShowDialog();
        }

        private void DataGridEmployeesMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployees.SelectedItem != null)
            {
                var employee = DataGridEmployees.SelectedItem as Employee;

                var window = new AdminAddUpdateEmployee(employee);
                window.ShowDialog();
            }
        }

        private void LoadEmployees()
        {
            employees = dB.GetEmployees();

            DataGridEmployees.ItemsSource = employees;

            FilterPatients();
        }

        private void FilterEmployees()
        {
            filterEmployees = employees;

            if (!string.IsNullOrEmpty(SearchEmployee.Text))
            {
                filterEmployees = filterEmployees.Where(p => p.lastName.ToLower().Contains(SearchEmployee.Text.ToLower()) || p.firstName.ToLower().Contains(SearchEmployee.Text.ToLower())
                                                || p.middleName.ToLower().Contains(SearchEmployee.Text.ToLower()) || p.post.ToLower().Contains(SearchEmployee.Text.ToLower())
                                                || p.postType.ToLower().Contains(SearchEmployee.Text.ToLower())).ToList();
            }

            if (FilterEmployee.SelectedItem != null && FilterEmployee.SelectedIndex != 0)
            {
                if(FilterEmployee.SelectedIndex == 1)
                {
                    filterEmployees = filterEmployees.Where(p => p.postType == "Обслуживающий").ToList();
                }
                else
                {
                    filterEmployees = filterEmployees.Where(p => p.postType == "Врачебный").ToList();
                }

                
            }

            DataGridEmployees.ItemsSource = filterEmployees;
            DataGridEmployees.Items.Refresh();


        }

        private void ButtonClickDeleteEmployee(object sender, RoutedEventArgs e)
        {

        }
    }
}
