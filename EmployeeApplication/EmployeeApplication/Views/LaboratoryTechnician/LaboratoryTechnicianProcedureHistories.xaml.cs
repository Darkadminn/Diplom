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
    /// Логика взаимодействия для LaboratoryTechnicianProcedureHistories.xaml
    /// </summary>
    public partial class LaboratoryTechnicianProcedureHistories : Window
    {
        DB dB = new DB();
        VisitHospitalProcedure visitHospitalProcedure0;
        List<VisitHospitalProcedureHistory> visitHospitalProcedureHistories = new List<VisitHospitalProcedureHistory>();
        public LaboratoryTechnicianProcedureHistories(VisitHospitalProcedure visitHospitalProcedure)
        {
            InitializeComponent();

            visitHospitalProcedureHistories = dB.GetVisitHospitalProcedureHistories(visitHospitalProcedure);
            DataGridProcedureHistories.ItemsSource = visitHospitalProcedureHistories;

            visitHospitalProcedure0 = visitHospitalProcedure;

            if (visitHospitalProcedure.count == visitHospitalProcedure.countСompleted)
            {
                ButtonSave.Visibility = Visibility.Collapsed;
            }

            ResultProcedureHistory.Text = visitHospitalProcedure.result;

            if (visitHospitalProcedure.type == "стационар" && visitHospitalProcedure.isResearche)
            {
                GridProcedure.Visibility = Visibility.Collapsed;
            }

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
                var procedureHistory = (VisitProcedureHistory)DataGridProcedureHistories.SelectedItem;

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

                if (visitHospitalProcedureHistories.Count + 1 > visitHospitalProcedure0.count)
                {
                    MessageBox.Show($"Количество проведений не может быть больше {visitHospitalProcedure0.count}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateProcedureHistory.SelectedDate?.Date.Add(timeProcedureHistory);

                if (dateTime >= visitHospitalProcedure0.date)
                {
                    MessageBox.Show("Дата проведения анализов не может быть больше даты назначения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                VisitHospitalProcedureHistory visitHospitalProcedureHistory = new VisitHospitalProcedureHistory
                {
                    date = (DateTime)dateTime,
                    visitHospitalProcedureId = visitHospitalProcedure0.id,
                };

                visitHospitalProcedureHistories.Add(visitHospitalProcedureHistory);

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

                    var procedureHistory = DataGridProcedureHistories.SelectedItem as VisitHospitalProcedureHistory;

                    int index = DataGridProcedureHistories.SelectedIndex;

                    if (dateTime >= visitHospitalProcedure0.date)
                    {
                        MessageBox.Show("Дата проведения процедуры не может быть больше даты приема", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    VisitHospitalProcedureHistory visitHospitalProcedureHistory = new VisitHospitalProcedureHistory
                    {
                        date = (DateTime)dateTime,
                        visitHospitalProcedureId = visitHospitalProcedure0.id,
                    };

                    visitHospitalProcedureHistories.Remove(procedureHistory);

                    visitHospitalProcedureHistories.Insert(index, visitHospitalProcedureHistory);

                    DataGridProcedureHistories.Items.Refresh();
                }
            }

        }

        private void ButtonClickDelete(object sender, RoutedEventArgs e)
        {
            var procedureHistory = DataGridProcedureHistories.SelectedItem as VisitHospitalProcedureHistory;
            visitHospitalProcedureHistories.Remove(procedureHistory);
            DataGridProcedureHistories.Items.Refresh();
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            if (GridProcedure.Visibility == Visibility.Collapsed) visitHospitalProcedureHistories = null;

            visitHospitalProcedure0.result = ResultProcedureHistory.Text;

            bool result = dB.SaveProcedure(visitHospitalProcedure0, visitHospitalProcedureHistories);

            if (result)
            {
                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK);
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
