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
    /// Логика взаимодействия для DoctorTreatmentProcedureHistories.xaml
    /// </summary>
    public partial class DoctorTreatmentProcedureHistories : Window
    {
        DB dB = new DB();
        HospitalProcedure treatmentProcedure0;
        List<HospitalProcedureHistory> treatmentProcedureHistories = new List<HospitalProcedureHistory>();
        public DoctorTreatmentProcedureHistories(HospitalProcedure treatmentProcedure, List<HospitalProcedureHistory> hospitalProcedureHistories)
        {
            InitializeComponent();

            treatmentProcedureHistories = hospitalProcedureHistories;
            DataGridProcedureHistories.ItemsSource = treatmentProcedureHistories;

            treatmentProcedure0 = treatmentProcedure;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void DataGridProcedureHistories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridProcedureHistories.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var procedureHistory = (HospitalProcedureHistory)DataGridProcedureHistories.SelectedItem;

                DateProcedureHistory.SelectedDate = procedureHistory.date;
                TimeProcedureHistory.Text = procedureHistory.date.ToString("HH mm").Replace(" ", "");
            }
        }

        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
            if (DateProcedureHistory.SelectedDate == null || TimeProcedureHistory.Text.Contains(TimeProcedureHistory.PromptChar.ToString()))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (!TimeSpan.TryParse(TimeProcedureHistory.Text, out TimeSpan timeProcedureHistory))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateProcedureHistory.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (treatmentProcedureHistories.Count + 1 > treatmentProcedure0.count)
                {
                    MessageBox.Show($"Количество прохождений не может быть больше {treatmentProcedure0.count}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateProcedureHistory.SelectedDate?.Date.Add(timeProcedureHistory);

                if (dateTime >= treatmentProcedure0.date)
                {
                    MessageBox.Show("Дата проведения процедуры не может быть больше даты назначения процедуры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                HospitalProcedureHistory treatmentProcedureHistory = new HospitalProcedureHistory
                {
                    date = (DateTime)dateTime,
                    hospitalProcedureId = treatmentProcedure0.id,
                };

                treatmentProcedureHistories.Add(treatmentProcedureHistory);

                DataGridProcedureHistories.Items.Refresh();
            }
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            if (DataGridProcedureHistories.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateProcedureHistory.SelectedDate == null || TimeProcedureHistory.Text.Contains(TimeProcedureHistory.PromptChar.ToString()))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    if (!TimeSpan.TryParse(TimeProcedureHistory.Text, out TimeSpan timeProcedureHistory))
                    {
                        MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateProcedureHistory.SelectedDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DateTime? dateTime = DateProcedureHistory.SelectedDate?.Date.Add(timeProcedureHistory);

                    var procedureHistory = DataGridProcedureHistories.SelectedItem as HospitalProcedureHistory;

                    int index = DataGridProcedureHistories.SelectedIndex;

                    if (dateTime >= treatmentProcedure0.date)
                    {
                        MessageBox.Show("Дата проведения процедуры не может быть больше даты назначения процедуры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    HospitalProcedureHistory treatmentProcedureHistory = new HospitalProcedureHistory
                    {
                        date = (DateTime)dateTime,
                        hospitalProcedureId = treatmentProcedure0.id,
                    };

                    treatmentProcedureHistories.Remove(procedureHistory);

                    treatmentProcedureHistories.Insert(index, treatmentProcedureHistory);

                    DataGridProcedureHistories.Items.Refresh();
                }
            }
        }

        private void ButtonClickDelete(object sender, RoutedEventArgs e)
        {
            var procedureHistory = DataGridProcedureHistories.SelectedItem as HospitalProcedureHistory;
            treatmentProcedureHistories.Remove(procedureHistory);
            DataGridProcedureHistories.Items.Refresh();
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            StaticHospitalProcedureHistory.hospitalProcedureHistories = treatmentProcedureHistories;

            MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK);
            this.DialogResult = true;
        }

        
    }
}
