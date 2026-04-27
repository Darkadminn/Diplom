using EmployeeWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        ExportPDF exportPDF = new ExportPDF();
        ExportExcel exportExcel = new ExportExcel();
        bool graphBool = false;
        public AdminWindow()
        {
            InitializeComponent();
            TextBlockUser.Text = UserAuthorization.fio;
            TextBlockPostUser.Text = UserAuthorization.postName;

            Doctors.ItemsSource = dB.GetEmployees().Where(emp => emp.postType == "Врачебный").GroupBy(emp => emp.id).Select(emp => emp.First()).ToList();
            DoctorsGraph.ItemsSource = dB.GetEmployees().Where(emp => emp.postType == "Врачебный").GroupBy(emp => emp.id).Select(emp => emp.First()).ToList();
            DoctorsProcedures.ItemsSource = dB.GetEmployees().Where(emp => emp.postType == "Врачебный").GroupBy(emp => emp.id).Select(emp => emp.First()).ToList();

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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.Height - 180 >= 30)
            {
                DataGridUsers.Height = this.Height - 180;
                DataGridEmployees.Height = this.Height - 180;
                DataGridEmployeeAssignments.Height = this.Height - 180;
                DataGridPatients.Height = this.Height - 180;
                DataGridVisits.Height = this.Height - 270;
                Graph.Height = this.Height - 270;
            }
            else
            {
                DataGridUsers.Height = 30;
                DataGridEmployees.Height = 30;
                DataGridEmployeeAssignments.Height = 30;
                DataGridPatients.Height = 30;
                DataGridVisits.Height = 30;
                Graph.Height = 30;
            }


            if (this.Height - 290 >= 30) StackExit.Height = this.Height - 290;
            else StackExit.Height = 30;
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
            UserAuthorization.wingId = -1;

            var window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void ButtonClickAddPatient(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdatePatient();
            window.ShowDialog();

            if(window.DialogResult == true)
            {
                LoadPatients();
            }
        }

        private void DataGridPatientsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataGridPatients.SelectedItem != null)
            {
                var patient = DataGridPatients.SelectedItem as Patient;

                var window = new AdminAddUpdatePatient(patient);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadPatients();
                }
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
            var result = MessageBox.Show("Вы точно хотите удалить пациента?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

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

            if (window.DialogResult == true)
            {
                LoadEmployees();
            }
        }

        private void DataGridEmployeesMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployees.SelectedItem != null)
            {
                var employee = DataGridEmployees.SelectedItem as Employee;

                var window = new AdminAddUpdateEmployee(employee);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadEmployees();
                }
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
            var result = MessageBox.Show("Вы точно хотите удалить сотрудника?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var employee = DataGridEmployees.SelectedItem as Employee;

                dB.DeleteEmployee(employee.id);

                LoadEmployees();
            }
        }

        private void ButtonClickAddEmployeeAssignment(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdateEmployeeAssignment();
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                LoadEmployeeAssignments();
            }
        }

        private void DataGridEmployeeAssignmentsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployeeAssignments.SelectedItem != null)
            {
                var employeeAssignment = DataGridEmployeeAssignments.SelectedItem as EmployeeAssignment;

                var window = new AdminAddUpdateEmployeeAssignment(employeeAssignment);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadEmployeeAssignments();
                }
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
            var result = MessageBox.Show("Вы точно хотите удалить назначение?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var employeeAssignment = DataGridEmployeeAssignments.SelectedItem as EmployeeAssignment;

                dB.DeleteEmployeeAssignment(employeeAssignment.id);

                LoadEmployeeAssignments();
            }
        }

        private void ButtonClickAddUser(object sender, RoutedEventArgs e)
        {
            var window = new AdminAddUpdateUser();
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                LoadUsers();
            }
        }

        private void DataGridUsersMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridUsers.SelectedItem != null)
            {
                var user = DataGridUsers.SelectedItem as User;

                var window = new AdminAddUpdateUser(user);
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    LoadUsers();
                }
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
            var result = MessageBox.Show("Вы точно хотите удалить пользователя?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var user = DataGridUsers.SelectedItem as User;

                dB.DeleteUser(user.userId, user.role);

                LoadUsers();
            }
        }

        private void LoadGraph()
        {
            Employee employee = DoctorsGraph.SelectedItem as Employee;

            List<Visit> visits = dB.GetVisitsFull(employee.id).Where(emp => emp.status == "Завершен").ToList();

            if (DateStart.SelectedDate > DateEnd.SelectedDate)
            {
                MessageBox.Show("Начало периода не может быть больше окончания периода", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Graph.Plot.Clear();
                graphBool = false;
                return;
            }

            if (DateStart.SelectedDate != null && DateEnd.SelectedDate == null) visits = visits.Where(v => v.date.Date >= DateStart.SelectedDate).ToList();
            else if (DateStart.SelectedDate == null && DateEnd.SelectedDate != null) visits = visits.Where(v => v.date.Date <= DateEnd.SelectedDate).ToList();
            else if (DateStart.SelectedDate != null && DateEnd.SelectedDate != null) visits = visits.Where(v => v.date.Date >= DateStart.SelectedDate && v.date.Date <= DateEnd.SelectedDate).ToList();

            Graph.Plot.Clear(); // Очищаем график перед построением нового

            if (visits.Count > 0)
            {
                /*var groupedVisits = visits
                .GroupBy(v => new { Year = v.dateTime.Year, Month = v.dateTime.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count(),
                    Label = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMM yyyy}"
                })
                .OrderBy(x => x.Date)
                .ToList();

                // Преобразуем даты в числовой формат для позиционирования
                double[] dates = groupedVisits.Select(x => x.Date.ToOADate()).ToArray();
                double[] counts = groupedVisits.Select(x => (double)x.Count).ToArray();
                string[] labels = groupedVisits.Select(x => x.Label).ToArray();*/

                var groupedVisitsByDay = visits
                .Where(v => v.date != null)
                .GroupBy(v => new
                {
                    Year = v.date.Year,
                    Month = v.date.Month,
                    Day = v.date.Day
                })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Count = g.Count(),
                    Label = $"{new DateTime(g.Key.Year, g.Key.Month, g.Key.Day):dd.MM.yyyy}",
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day
                })
                .OrderBy(x => x.Date)
                .ToList();

                // Для графика
                double[] dates = groupedVisitsByDay.Select(x => x.Date.ToOADate()).ToArray();
                double[] counts = groupedVisitsByDay.Select(x => (double)x.Count).ToArray();
                string[] labels = groupedVisitsByDay.Select(x => x.Label).ToArray();

                // Создаем столбчатую диаграмму
                var bars = Graph.Plot.Add.Bars(dates, counts);

                // Настраиваем подписи по оси X
                Graph.Plot.Axes.DateTimeTicksBottom();

                // Форматирование подписей дат
                Graph.Plot.Axes.Bottom.Label.Text = "Дата";
                // Graph.Plot.Axes.Bottom.TickLabelFormat("MMM yyyy", dateTimeFormat: true);

                Graph.Plot.Axes.DateTimeTicksBottom();
                // Подписи осей
                Graph.Plot.Title($"Статистика приемов: {employee.fio}");
                Graph.Plot.YLabel("Количество приемов");

                // Настройки отображения
                /*Graph.Plot.Grid(true);
                Graph.Plot.SetAxisLimits(yMin: 0);
                Graph.Plot.AutoScale();*/

                graphBool = true;

            }
            else
            {
                // Если нет данных, показываем сообщение
                Graph.Plot.Title("Нет данных для отображения");
                Graph.Plot.XLabel("Период (месяц)");
                Graph.Plot.YLabel("Количество завершенных приемов");

                graphBool = false;
            }

            Graph.Refresh();
        }

        private void LoadTable()
        {
            Employee employee = Doctors.SelectedItem as Employee;

            List<Visit> visits = dB.GetVisitsFull(employee.id).Where(emp => emp.status == "Завершен").ToList();

            if (DateVisitStart.SelectedDate > DateVisitEnd.SelectedDate)
            {
                DataGridVisits.ItemsSource = null;
                DataGridVisits.Items.Refresh();
                return;
            }

            if (DateVisitStart.SelectedDate != null && DateVisitEnd.SelectedDate == null) visits = visits.Where(v => v.date.Date >= DateVisitStart.SelectedDate).ToList();
            else if (DateVisitStart.SelectedDate == null && DateVisitEnd.SelectedDate != null) visits = visits.Where(v => v.date.Date <= DateVisitEnd.SelectedDate).ToList();
            else if (DateVisitStart.SelectedDate != null && DateVisitEnd.SelectedDate != null) visits = visits.Where(v => v.date.Date >= DateVisitStart.SelectedDate && v.date.Date <= DateVisitEnd.SelectedDate).ToList();


            DataGridVisits.ItemsSource = visits;
            DataGridVisits.Items.Refresh();
        }

        private void LoadTableServices()
        {
            Employee employee = DoctorsProcedures.SelectedItem as Employee;

            List<VisitHospitalProcedure> visitHospitals = dB.GetVisitHospitalProceduresAll().Where(v => v.employeeId == employee.id).ToList();

            if (DateProcedureStart.SelectedDate > DateProcedureEnd.SelectedDate)
            {
                DataGridProcedures.ItemsSource = null;
                DataGridProcedures.Items.Refresh();
                return;
            }

            if (DateProcedureStart.SelectedDate != null && DateProcedureEnd.SelectedDate == null) visitHospitals = visitHospitals.Where(v => v.date.Date >= DateProcedureStart.SelectedDate).ToList();
            else if (DateProcedureStart.SelectedDate == null && DateProcedureEnd.SelectedDate != null) visitHospitals = visitHospitals.Where(v => v.date.Date <= DateProcedureEnd.SelectedDate).ToList();
            else if (DateProcedureStart.SelectedDate != null && DateProcedureEnd.SelectedDate != null) visitHospitals = visitHospitals.Where(v => v.date.Date >= DateProcedureStart.SelectedDate && v.date.Date <= DateProcedureEnd.SelectedDate).ToList();


            DataGridProcedures.ItemsSource = visitHospitals;
            DataGridProcedures.Items.Refresh();
        }

        private void Doctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Doctors.SelectedIndex != -1)
            {
                LoadTable();
            }
        }

        private void DoctorsGraph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsGraph.SelectedIndex != -1)
            {
                LoadGraph();
            }
        }

        private void DateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsGraph.SelectedIndex != -1)
            {
                LoadGraph();
            }
        }

        private void DateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsGraph.SelectedIndex != -1)
            {
                LoadGraph();
            }
        }

        private void DateVisitStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Doctors.SelectedIndex != -1)
            {
                LoadTable();
            }
        }

        private void DateVisitEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Doctors.SelectedIndex != -1)
            {
                LoadTable();
            }
        }

        private void DoctorsProcedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsProcedures.SelectedIndex != -1)
            {
                LoadTableServices();
            }
        }

        private void DateProcedureStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsProcedures.SelectedIndex != -1)
            {
                LoadTableServices();
            }
        }

        private void DateProcedureEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsProcedures.SelectedIndex != -1)
            {
                LoadTableServices();
            }
        }

        private void ButtonClickExport(object sender, RoutedEventArgs e)
        {
            if (Formats.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите формат", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TabControlTable.SelectedIndex == 0)
            {
                if (DataGridVisits.Items.Count > 0)
                {
                    DataGrid dataGrid = new DataGrid
                    {
                        AutoGenerateColumns = false,
                        Margin = new Thickness(5)
                    };

                    // Добавляем столбцы
                    dataGrid.Columns.Add(CreateColumn("Дата", "date", "dd.MM.yyyy", 100));
                    dataGrid.Columns.Add(CreateColumn("Время", "date", "HH:mm", 80));
                    dataGrid.Columns.Add(CreateColumn("Пациент", "patient", null, 150));
                    dataGrid.Columns.Add(CreateColumn("Статус приема", "status", null, 100));
                    dataGrid.Columns.Add(CreateColumn("Состояние пациента", "objective", null, 200));
                    dataGrid.Columns.Add(CreateColumn("Жалобы", "subjective", null, 200));
                    dataGrid.Columns.Add(CreateColumn("Диагноз", "diagnosis", null, 150));
                    dataGrid.Columns.Add(CreateColumn("Рекомендации", "recommendation", null, 200));

                    dataGrid.ItemsSource = DataGridVisits.ItemsSource;

                    if (Formats.SelectedIndex == 0)
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            exportExcel.ExportToExcel(dataGrid, saveFileDialog.FileName);
                        }
                    }
                    else
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            exportPDF.ExportToPdf(dataGrid, saveFileDialog.FileName, "Приемы");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("В таблице нету записей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
            else if (TabControlTable.SelectedIndex == 1)
            {
                if (graphBool == true)
                {
                    if (Formats.SelectedIndex == 0)
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            byte[] graphImage = SaveGraphToPng();

                            if (graphImage == null)
                            {
                                MessageBox.Show("Не удалось получить изображение графика", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            exportExcel.ExportGraphToExcel(graphImage, saveFileDialog.FileName);
                        }
                    }
                    else
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            byte[] graphImage = SaveGraphToPng();

                            if (graphImage == null)
                            {
                                MessageBox.Show("Не удалось получить изображение графика", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            exportPDF.ExportGraphToPdf(graphImage, saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("В графике нету записей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (TabControlTable.SelectedIndex == 2)
            {
                if (DataGridProcedures.Items.Count > 0)
                {
                    DataGrid dataGrid = new DataGrid
                    {
                        AutoGenerateColumns = false,
                        Margin = new Thickness(5)
                    };

                    // Добавляем столбцы
                    dataGrid.Columns.Add(CreateColumn("Дата", "date", "dd.MM.yyyy", 100));
                    dataGrid.Columns.Add(CreateColumn("Пациент", "patient", null, 150));
                    dataGrid.Columns.Add(CreateColumn("Услуга", "medicalService", null, 250));
                    dataGrid.Columns.Add(CreateColumn("Тип", "type", null, 200));
                    dataGrid.Columns.Add(CreateColumn("Количество", "count", null, 100));
                    dataGrid.Columns.Add(CreateColumn("Количество пройденных", "countСompleted", null, 100));

                    dataGrid.ItemsSource = DataGridProcedures.ItemsSource;

                    if (Formats.SelectedIndex == 0)
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            exportExcel.ExportToExcel(dataGrid, saveFileDialog.FileName);
                        }
                    }
                    else
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            exportPDF.ExportToPdf(dataGrid, saveFileDialog.FileName, "Услуги");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("В таблице нету записей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
        }


        private DataGridTextColumn CreateColumn(string header, string bindingPath, string stringFormat, double width)
        {
            DataGridTextColumn column = new DataGridTextColumn
            {
                Header = header,
                Width = new DataGridLength(width)
            };

            Binding binding = new Binding(bindingPath);
            if (!string.IsNullOrEmpty(stringFormat))
            {
                binding.StringFormat = stringFormat;
                binding.ConverterCulture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
            }

            column.Binding = binding;

            return column;
        }

        // Метод для сохранения графика в PNG
        private byte[] SaveGraphToPng()
        {
            if (!graphBool)
                return null;

            // Устанавливаем размер изображения (ширина, высота)
            int width = 800;
            int height = 600;

            // Сохраняем график в массив байтов
            byte[] imageBytes = Graph.Plot.GetImageBytes(width, height);
            return imageBytes;
        }

        
    }
}
