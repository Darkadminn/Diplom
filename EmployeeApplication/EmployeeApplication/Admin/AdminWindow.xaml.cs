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
        List<EmployeeAssignment> employeeAssignments = new List<EmployeeAssignment>();
        List<EmployeeAssignment> filterEmployeeAssignments = new List<EmployeeAssignment>();
        List<User> users = new List<User>();
        List<User> filterUsers = new List<User>();
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
            FilterUser.ItemsSource = wingsAll;

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

            SearchEmployeeAssignment.TextChanged += (a, args) =>
            {
                FilterEmployeeAssignments();
            };

            FilterEmployeeAssignment.SelectionChanged += (a, args) =>
            {
                FilterEmployeeAssignments();
            };

            SearchUser.TextChanged += (a, args) =>
            {
                FilterUsers();
            };

            FilterUser.SelectionChanged += (a, args) =>
            {
                FilterUsers();
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

                StackPanelSearchEmployeeAssignment.Margin = new Thickness(0, 0, margin, 0);
                StackPanelFilterEmployeeAssignment.Margin = new Thickness(0, 0, margin, 0);
                DataGridEmployeeAssignments.Width = this.Width - 155;

                StackPanelSearchUser.Margin = new Thickness(0, 0, margin, 0);
                StackPanelFilterUser.Margin = new Thickness(0, 0, margin, 0);
                DataGridUsers.Width = this.Width - 155;
            }

            if (ScrollViewerWindow.Height > 100)
            {
                DataGridPatients.Height = ScrollViewerWindow.Height - 58;
                DataGridEmployees.Height = ScrollViewerWindow.Height - 58;
                DataGridEmployeeAssignments.Height = ScrollViewerWindow.Height - 58;
                DataGridUsers.Height = ScrollViewerWindow.Height - 58;
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

            LoadEmployeeAssignments();
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

            LoadUsers();
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
            var result = MessageBox.Show("Вы точно хотите удалить пациента?", "Предупреждение", MessageBoxButton.YesNo);

            if(result == MessageBoxResult.Yes)
            {
                var patient = DataGridPatients.SelectedItem as Patient;

                dB.DeletePatient(patient.id);

                LoadPatients();
            }
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

            FilterEmployees();
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

        private void ButtonClickAddEmployeeAssignment(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdateEmployeeAssignment();
            window.ShowDialog();
        }

        private void DataGridEmployeeAssignmentsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployeeAssignments.SelectedItem != null)
            {
                var employeeAssignment = DataGridEmployeeAssignments.SelectedItem as EmployeeAssignment;

                var window = new AdminAddUpdateEmployeeAssignment(employeeAssignment);
                window.ShowDialog();
            }
        }

        private void LoadEmployeeAssignments()
        {
            employeeAssignments = dB.GetEmployeeAssignments();

            DataGridEmployeeAssignments.ItemsSource = employeeAssignments;

            FilterEmployeeAssignments();
        }

        private void FilterEmployeeAssignments()
        {
            filterEmployeeAssignments = employeeAssignments;

            if (!string.IsNullOrEmpty(SearchEmployeeAssignment.Text))
            {
                filterEmployeeAssignments = filterEmployeeAssignments.Where(p => p.lastName.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower()) || p.firstName.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower())
                                                || p.middleName.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower()) || p.post.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower())
                                                || p.cabinet.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower()) || p.wing.ToLower().Contains(SearchEmployeeAssignment.Text.ToLower())).ToList();
            }

            if (FilterEmployeeAssignment.SelectedItem != null && FilterEmployeeAssignment.SelectedIndex != 0)
            {
                if (FilterEmployeeAssignment.SelectedIndex == 1)
                {
                    filterEmployeeAssignments = filterEmployeeAssignments.Where(p => p.postType == "Обслуживающий").ToList();
                }
                else
                {
                    filterEmployeeAssignments = filterEmployeeAssignments.Where(p => p.postType == "Врачебный").ToList();
                }


            }

            DataGridEmployeeAssignments.ItemsSource = filterEmployeeAssignments;
            DataGridEmployeeAssignments.Items.Refresh();


        }

        private void ButtonClickDeleteEmployeeAssignment(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickAddUser(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdateUser();
            window.ShowDialog();
        }

        private void DataGridUsersMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridUsers.SelectedItem != null)
            {
                var user = DataGridUsers.SelectedItem as User;

                var window = new AdminAddUpdateUser(user);
                window.ShowDialog();
            }
        }

        private void LoadUsers()
        {
            users = dB.GetUsers();

            DataGridUsers.ItemsSource = users;

            FilterUsers();
        }

        private void FilterUsers()
        {
            filterUsers = users;

            if (!string.IsNullOrEmpty(SearchUser.Text))
            {
                filterUsers = filterUsers.Where(p => p.lastName.ToLower().Contains(SearchUser.Text.ToLower()) || p.firstName.ToLower().Contains(SearchUser.Text.ToLower())
                                                || p.middleName.ToLower().Contains(SearchUser.Text.ToLower()) || p.wing.ToLower().Contains(SearchUser.Text.ToLower())
                                                || p.post.ToLower().Contains(SearchUser.Text.ToLower()) || p.role.ToLower().Contains(SearchUser.Text.ToLower())
                                                || p.login.ToLower().Contains(SearchUser.Text.ToLower())).ToList();
            }

            if (FilterUser.SelectedItem != null && FilterUser.SelectedIndex != 0)
            {
                var filter = FilterUser.SelectedItem as Wing;

                filterUsers = filterUsers.Where(p => p.wingId == filter.id).ToList();
            }

            DataGridUsers.ItemsSource = filterUsers;
            DataGridUsers.Items.Refresh();


        }

        private void ButtonClickDeleteUser(object sender, RoutedEventArgs e)
        {

        }


    }
}
