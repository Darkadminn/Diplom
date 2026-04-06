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
    /// Логика взаимодействия для AdminAddUpdateEmployeeAssignment.xaml
    /// </summary>
    public partial class AdminAddUpdateEmployeeAssignment : Window
    {
        DB dB = new DB();
        List<WorkSchedule> workSchedules = new List<WorkSchedule>();
        List<Wing> wings = new List<Wing>();
        List<Employee> employees = new List<Employee>();
        List<Department> departments = new List<Department>();
        List<Cabinet> cabinets = new List<Cabinet>();
        List<Department> filterDepartments = new List<Department>();
        List<Cabinet> filterCabinets = new List<Cabinet>();
        EmployeeAssignment employeeAssignment0 = new EmployeeAssignment();
        public AdminAddUpdateEmployeeAssignment()
        {
            InitializeComponent();

            ButtonOperation.Content = "Добавить";
            ButtonOperation.Click += ButtonClickAdd;

            TextBlockHeader.Text = "Добавление назначения";
            this.Title = "ИС «МедСервис» - Добавление назначения";

            employees = dB.GetEmployees().Where(e => e.dateDismissal == null).ToList();
            wings = dB.GetWings();
            departments = dB.GetDepartments();
            cabinets = dB.GetCabinets();

            filterDepartments = departments;
            filterCabinets = cabinets;

            Employees.ItemsSource = employees;
            Wings.ItemsSource = wings;
            Departments.ItemsSource = filterDepartments;
            Cabinets.ItemsSource = filterCabinets;

            Departments.IsEnabled = false;
            Cabinets.IsEnabled = false;

            DataGridWorkSchedules.ItemsSource = workSchedules;
        }

        public AdminAddUpdateEmployeeAssignment(EmployeeAssignment employeeAssignment)
        {
            InitializeComponent();

            ButtonOperation.Content = "Сохранить";
            ButtonOperation.Click += ButtonClickUpdate;

            TextBlockHeader.Text = "Редактирование назначения";
            this.Title = "ИС «МедСервис» - Редактирование назначения";

            employees = dB.GetEmployees();
            wings = dB.GetWings();
            departments = dB.GetDepartments();
            cabinets = dB.GetCabinets();

            filterDepartments = departments;
            filterCabinets = cabinets;

            Employees.ItemsSource = employees;
            Wings.ItemsSource = wings;
            Departments.ItemsSource = filterDepartments;
            Cabinets.ItemsSource = filterCabinets;

            Employees.SelectedItem = employees.FirstOrDefault(e => e.id == employeeAssignment.employeeId);
            Wings.SelectedItem = wings.FirstOrDefault(w => w.id == employeeAssignment.wingId);
            Departments.SelectedItem = filterDepartments.FirstOrDefault(d => d.id == employeeAssignment.departmentId);
            Cabinets.SelectedItem = filterCabinets.FirstOrDefault(c => c.id == employeeAssignment.cabinetId);

            if(employeeAssignment.type == "Основное")
            {
                Types.SelectedIndex = 0;
            }
            else
            {
                Types.SelectedIndex = 1;
            }

            Types.IsEnabled = false;
            Employees.IsEnabled = false;

            DateFrom.SelectedDate = employeeAssignment.dateFrom;
            DateTo.SelectedDate = employeeAssignment.dateTo;

            employeeAssignment0 = employeeAssignment;

            workSchedules = dB.GetWorkSchedules(employeeAssignment.id);

            DataGridWorkSchedules.ItemsSource = workSchedules;

        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {           
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }          
            
        }

        private void Wings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Wings.SelectedItem != null)
            {
                var wing = Wings.SelectedItem as Wing;

                filterDepartments = departments.Where(d => d.wingId == wing.id).ToList();

                Departments.ItemsSource = filterDepartments;
                Departments.Items.Refresh();

                Departments.IsEnabled = true;
                Cabinets.IsEnabled = false;
                Cabinets.SelectedItem = null;
            }
        }

        private void Departments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Departments.SelectedItem != null)
            {
                var department = Departments.SelectedItem as Department;

                filterCabinets = cabinets.Where(c => c.departmentId == department.id).ToList();

                Cabinets.ItemsSource = filterCabinets;
                Cabinets.Items.Refresh();

                Cabinets.IsEnabled = true;
            }
        }

        private void DataGridWorkSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataGridWorkSchedules.SelectedItem != null)
            {
                var workSchedule = DataGridWorkSchedules.SelectedItem as WorkSchedule;

                TimeStart.Text = workSchedule.timeStart.ToString().Replace(":", "").Remove(4,2);
                TimeEnd.Text = workSchedule.timeEnd.ToString().Replace(":", "").Remove(4, 2);

                WeekDays.SelectedItems.Clear();

                foreach (var selectedItem in WeekDays.Items)
                {
                    ListBoxItem itemContainer = WeekDays.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                    if (itemContainer != null && itemContainer.Content != null && workSchedule.weekDays.Contains(itemContainer.Content.ToString()))
                    {
                        WeekDays.SelectedItems.Add(selectedItem);
                    }
                }
            }
        }

        private void ButtonClickAddWorkSchedule(object sender, RoutedEventArgs e)
        {
            if (TimeStart.Text.Contains(TimeStart.PromptChar.ToString()) || TimeEnd.Text.Contains(TimeEnd.PromptChar.ToString()))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (WeekDays.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите дни недели", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TimeSpan.TryParse(TimeStart.Text, out TimeSpan timeStart))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TimeSpan.TryParse(TimeEnd.Text, out TimeSpan timeEnd))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeStart >= timeEnd)
                {
                    MessageBox.Show("Начало смены не может быть позже окончания смены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeStart < TimeSpan.FromHours(8))
                {
                    MessageBox.Show("Начала смены не может быть раньше 08:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd > TimeSpan.FromHours(20))
                {
                    MessageBox.Show("Окончание смены не может быть позже 20:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd - timeStart < TimeSpan.FromHours(2))
                {
                    MessageBox.Show("Смена не может длится меньше 2 часов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd - timeStart > TimeSpan.FromHours(5))
                {
                    MessageBox.Show("Смена не может длится больше 5 часов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (workSchedules.Count == 0)
                {

                    var selectedContents = new List<string>();
                    foreach (var selectedItem in WeekDays.SelectedItems)
                    {
                        ListBoxItem itemContainer = WeekDays.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                        if (itemContainer != null && itemContainer.Content != null)
                        {
                            selectedContents.Add(itemContainer.Content.ToString());
                        }
                    }

                    WorkSchedule workSchedule = new WorkSchedule()
                    {
                        timeStart = timeStart,
                        timeEnd = timeEnd,
                        weekDays = selectedContents.ToArray(),
                    };

                    workSchedules.Add(workSchedule);
                    DataGridWorkSchedules.Items.Refresh();
                }
                else
                {
                    var selectedContents = new List<string>();
                    foreach (var selectedItem in WeekDays.SelectedItems)
                    {
                        ListBoxItem itemContainer = WeekDays.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                        if (itemContainer != null && itemContainer.Content != null)
                        {
                            selectedContents.Add(itemContainer.Content.ToString());
                        }
                    }

                    bool weekDayAvailable = true;

                    foreach (var workSchedule in workSchedules)
                    {

                        foreach (string day in workSchedule.weekDays)
                        {
                            if (selectedContents.Contains(day))
                            {
                                weekDayAvailable = false;
                                break;
                            }
                        }

                        if (weekDayAvailable == false) break;
                    }

                    if (weekDayAvailable == false)
                    {
                        MessageBox.Show("Эти дни недели уже присутствуют", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {

                        WorkSchedule workSchedule = new WorkSchedule()
                        {
                            timeStart = timeStart,
                            timeEnd = timeEnd,
                            weekDays = selectedContents.ToArray(),
                        };

                        workSchedules.Add(workSchedule);
                        DataGridWorkSchedules.Items.Refresh();
                    }
                }
            }
        }

        private void ButtonClickUpdateWorkSchedule(object sender, RoutedEventArgs e)
        {
            if (TimeStart.Text.Contains(TimeStart.PromptChar.ToString()) || TimeEnd.Text.Contains(TimeEnd.PromptChar.ToString()))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (WeekDays.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите дни недели", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TimeSpan.TryParse(TimeStart.Text, out TimeSpan timeStart))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TimeSpan.TryParse(TimeEnd.Text, out TimeSpan timeEnd))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeStart >= timeEnd)
                {
                    MessageBox.Show("Начало смены не может быть позже окончания смены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeStart < TimeSpan.FromHours(8))
                {
                    MessageBox.Show("Начала смены не может быть раньше 08:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd > TimeSpan.FromHours(20))
                {
                    MessageBox.Show("Окончание смены не может быть позже 20:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd - timeStart < TimeSpan.FromHours(2))
                {
                    MessageBox.Show("Смена не может длится меньше 2 часов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (timeEnd - timeStart > TimeSpan.FromHours(5))
                {
                    MessageBox.Show("Смена не может длится больше 5 часов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if(DataGridWorkSchedules.SelectedItem != null)
                {
                    var oldWorkSchedule = DataGridWorkSchedules.SelectedItem as WorkSchedule;
                    int index = DataGridWorkSchedules.SelectedIndex;

                    workSchedules.Remove(oldWorkSchedule);

                    var selectedContents = new List<string>();
                    foreach (var selectedItem in WeekDays.SelectedItems)
                    {
                        ListBoxItem itemContainer = WeekDays.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                        if (itemContainer != null && itemContainer.Content != null)
                        {
                            selectedContents.Add(itemContainer.Content.ToString());
                        }
                    }

                    bool weekDayAvailable = true;

                    foreach (var workSchedule in workSchedules)
                    {

                        foreach (string day in workSchedule.weekDays)
                        {
                            if (selectedContents.Contains(day))
                            {
                                weekDayAvailable = false;
                                break;
                            }
                        }

                        if (weekDayAvailable == false) break;
                    }

                    if (weekDayAvailable == false)
                    {
                        MessageBox.Show("Эти дни недели уже присутствуют", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        workSchedules.Insert(index, oldWorkSchedule);
                        DataGridWorkSchedules.Items.Refresh();
                        return;
                    }
                    else
                    {

                        WorkSchedule workSchedule = new WorkSchedule()
                        {
                            timeStart = timeStart,
                            timeEnd = timeEnd,
                            weekDays = selectedContents.ToArray(),
                        };

                        workSchedules.Insert(index, workSchedule);
                        DataGridWorkSchedules.Items.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
          
            }
        }

        private void ButtonClickDeleteWorkSchedule(object sender, RoutedEventArgs e)
        {
            if (DataGridWorkSchedules.SelectedItem != null)
            {
                var workSchedule = DataGridWorkSchedules.SelectedItem as WorkSchedule;
                workSchedules.Remove(workSchedule);
                DataGridWorkSchedules.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
            if (Employees.SelectedItem == null || Types.SelectedIndex == -1 || DateFrom.SelectedDate == null || Wings.SelectedItem == null
                || Departments.SelectedItem == null || Cabinets.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateFrom.SelectedDate > DateTime.Now || (DateTo.SelectedDate > DateTime.Now && DateTo.SelectedDate != null))
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateFrom.SelectedDate > DateTo.SelectedDate && DateTo.SelectedDate != null)
                {
                    MessageBox.Show("Дата начала не может быть больше даты окончания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var employee = Employees.SelectedItem as Employee;
                var wing = Wings.SelectedItem as Wing;
                var department = Departments.SelectedItem as Department;
                var cabinet = Cabinets?.SelectedItem as Cabinet;
                string type;

                if(Types.SelectedIndex == 0)
                {
                    type = "Основное";
                }
                else
                {
                    type = "Совмещение";
                }

                var employeeAssignment = new EmployeeAssignment
                {
                    employeeId = employee.id,
                    wingId = wing.id,
                    departmentId = department.id,
                    cabinetId = cabinet.id,
                    dateFrom = (DateTime)DateFrom.SelectedDate,
                    dateTo = DateTo.SelectedDate,
                    type = type
                };

                bool result = dB.AddEmployeeAssignment(employeeAssignment, workSchedules);

                if (result == true)
                {
                    MessageBox.Show("Назначение успешно добавлено", "Успех", MessageBoxButton.OK);
                    this.DialogResult = true;
                    this.Close();
                }

            }
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            if (Employees.SelectedItem == null || Types.SelectedIndex == -1 || DateFrom.SelectedDate == null || Wings.SelectedItem == null
                || Departments.SelectedItem == null || Cabinets.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateFrom.SelectedDate > DateTime.Now || (DateTo.SelectedDate > DateTime.Now && DateTo.SelectedDate != null))
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateFrom.SelectedDate > DateTo.SelectedDate && DateTo.SelectedDate != null)
                {
                    MessageBox.Show("Дата начала не может быть больше даты окончания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var employee = Employees.SelectedItem as Employee;
                var wing = Wings.SelectedItem as Wing;
                var department = Departments.SelectedItem as Department;
                var cabinet = Cabinets?.SelectedItem as Cabinet;
                string type;

                if (Types.SelectedIndex == 0)
                {
                    type = "Основное";
                }
                else
                {
                    type = "Совмещение";
                }

                var employeeAssignment = new EmployeeAssignment
                {
                    employeeId = employee.id,
                    wingId = wing.id,
                    departmentId = department.id,
                    cabinetId = cabinet.id,
                    dateFrom = (DateTime)DateFrom.SelectedDate,
                    dateTo = DateTo.SelectedDate,
                    type = type,
                    id = employeeAssignment0.id
                };

                bool result = dB.UpdateEmployeeAssignment(employeeAssignment, workSchedules);

                if (result == true)
                {
                    MessageBox.Show("Данные успешно обновлены", "Успех", MessageBoxButton.OK);
                    this.DialogResult = true;
                }

            }
        }

        
    }
}
