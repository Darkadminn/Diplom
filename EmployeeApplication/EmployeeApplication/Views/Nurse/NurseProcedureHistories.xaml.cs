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
    /// Логика взаимодействия для NurseProcedureHistories.xaml
    /// </summary>
    public partial class NurseProcedureHistories : Window
    {
        DB dB = new DB();
        VisitProcedure visitProcedure0;
        List<VisitProcedureHistory> visitProcedureHistories = new List<VisitProcedureHistory>();
        public NurseProcedureHistories(VisitProcedure visitProcedure)
        {
            InitializeComponent();

            visitProcedureHistories = dB.GetVisitProcedureHistories(visitProcedure.id);
            DataGridProcedureHistories.ItemsSource = visitProcedureHistories;

            visitProcedure0 = visitProcedure;
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
            if(DataGridProcedureHistories.SelectedItem == null)
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

                if(visitProcedureHistories.Count + 1 > visitProcedure0.count)
                {
                    MessageBox.Show($"Количество прохождений не может быть больше {visitProcedure0.count}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateProcedureHistory.SelectedDate?.Date.Add(timeProcedureHistory);

                if(dateTime >= visitProcedure0.visitDate)
                {
                    MessageBox.Show("Дата проведения процедуры не может быть больше даты приема", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                VisitProcedureHistory visitProcedureHistory = new VisitProcedureHistory
                {
                    date = (DateTime)dateTime,
                    visitProcedureId = visitProcedure0.id,
                };

                visitProcedureHistories.Add(visitProcedureHistory);

                DataGridProcedureHistories.Items.Refresh();
            }
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {

            if(DataGridProcedureHistories.SelectedItem == null)
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

                    var procedureHistory = DataGridProcedureHistories.SelectedItem as VisitProcedureHistory;

                    int index = DataGridProcedureHistories.SelectedIndex;

                    if (dateTime >= visitProcedure0.visitDate)
                    {
                        MessageBox.Show("Дата проведения процедуры не может быть больше даты приема", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    VisitProcedureHistory visitProcedureHistory = new VisitProcedureHistory
                    {
                        date = (DateTime)dateTime,
                        visitProcedureId = visitProcedure0.id,
                    };

                    visitProcedureHistories.Remove(procedureHistory);

                    visitProcedureHistories.Insert(index, visitProcedureHistory);

                    DataGridProcedureHistories.Items.Refresh();
                }
            }
            
        }

        private void ButtonClickDelete(object sender, RoutedEventArgs e)
        {
            var procedureHistory = DataGridProcedureHistories.SelectedItem as VisitProcedureHistory;
            visitProcedureHistories.Remove(procedureHistory);
            DataGridProcedureHistories.Items.Refresh();
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            bool result = dB.SaveVisitProcedure(visitProcedure0, visitProcedureHistories);

            if (result)
            {
                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK);
                this.DialogResult = true;
            }
        }

        
    }
}
