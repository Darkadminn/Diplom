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
    /// Логика взаимодействия для DoctorAddUpdateVisit.xaml
    /// </summary>
    public partial class DoctorAddUpdateVisit : Window
    {
        DB dB = new DB();
        Visit visit0 = new Visit();
        List<VisitProcedure> procedures = new List<VisitProcedure>();
        List<MedicalService> medicalProcedures = new List<MedicalService>();
        bool treatment;
        public DoctorAddUpdateVisit(Visit visit, bool read)
        {
            InitializeComponent();

            DateVisit.SelectedDate = visit.date.Date;
            TimeVisit.Text = visit.date.ToString("hh:mm");
            Patient.Text = visit.patient;

            medicalProcedures = dB.GetMedicalProcedures().Where(m => !m.isOperation).ToList();
            Service.ItemsSource = procedures;

            DateVisit.IsEnabled = false;
            TimeVisit.IsEnabled = false;
            Patient.IsEnabled = false;

            if(read == true )
            {
                Subjective.Text = visit.subjective;
                Objective.Text = visit.objective;
                Diagnosis.Text = visit.diagnosis;
                Recommendation.Text = visit.recommendation;

                procedures = dB.GetCurrentVisitProcedures(visit.id);

                DataGridServices.ItemsSource = procedures;

                if(visit.isTreatment == true )
                {
                    DirectionBool.IsChecked = true;
                }

                Subjective.IsEnabled = false;
                Objective.IsEnabled = false;
                Diagnosis.IsEnabled = false;
                Recommendation.IsEnabled = false;
                Service.IsEnabled = false;
                Count.IsEnabled = false;
                Comment.IsEnabled = false;
                DirectionBool.IsEnabled = false;

                ButtonAdd.IsEnabled = false;
                ButtonUpdate.IsEnabled = false;
                ButtonSave.IsEnabled = false;
                ButtonSave.Visibility = Visibility.Hidden;

                DataGridServices.IsEnabled = false;

                visit0 = visit;
            }

            treatment = false;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Service.SelectedItem != null)
            {
                var service = Service.SelectedItem as MedicalService;
                
                if(service.isAnalisis == true)
                {
                    Count.Value = 1;
                    Count.IsEnabled = false;
                }
                else
                {
                    Count.IsEnabled = true;
                }
            }
        }

        private void DirectionBool_Checked(object sender, RoutedEventArgs e)
        {
            treatment = true;
        }

        private void DirectionBool_Unchecked(object sender, RoutedEventArgs e)
        {
            treatment = false;
        }

        private void DataGridServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataGridServices.SelectedItem != null)
            {
                var procedure = DataGridServices.SelectedItem as VisitProcedure;

                Service.SelectedItem = medicalProcedures.FirstOrDefault(mp => mp.id == procedure.medicalServiceId);
                Count.Text = procedure.count.ToString();
                Comment.Text = procedure.comment;
            }
        }

        private void ButtonClickAddProcedure(object sender, RoutedEventArgs e)
        {
            if(Service.SelectedItem != null && !String.IsNullOrEmpty(Count.Text))
            {
                int count = int.Parse(Count.Text);

                if(count > 0)
                {
                    var procedure = Service.SelectedItem as MedicalService;
                    string comment = Comment.Text;

                    procedures.Add(new VisitProcedure
                    {
                        medicalServiceId = procedure.id,
                        medicalService = procedure.name,
                        count = count,
                        comment = comment
                    });

                    DataGridServices.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Количество должно быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Заполните поля процедура и количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonClickUpdateProcedure(object sender, RoutedEventArgs e)
        {
            if (DataGridServices.SelectedItem != null)
            {
                if (Service.SelectedItem != null && !String.IsNullOrEmpty(Count.Text))
                {
                    int count = int.Parse(Count.Text);

                    if (count > 0)
                    {
                        var procedure = Service.SelectedItem as MedicalService;
                        string comment = Comment.Text;
                        var visitProcedure = DataGridServices.SelectedItem as VisitProcedure;
                        int index = DataGridServices.SelectedIndex;

                        procedures.Remove(visitProcedure);

                        procedures.Insert(index, new VisitProcedure
                        {
                            medicalServiceId = procedure.id,
                            medicalService = procedure.name,
                            count = count,
                            comment = comment
                        });

                        DataGridServices.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Количество должно быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Заполните поля процедура и количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ButtonClickDeleteService(object sender, RoutedEventArgs e)
        {
            if(DataGridServices.SelectedItem != null)
            {
                var visitProcedure = DataGridServices.SelectedItem as VisitProcedure;
                procedures.Remove(visitProcedure);
                DataGridServices.Items.Refresh();
            }
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            visit0.objective = Objective.Text;
            visit0.subjective = Subjective.Text;
            visit0.diagnosis = Diagnosis.Text;

            if (!string.IsNullOrWhiteSpace(Recommendation.Text)) visit0.recommendation = Recommendation.Text;
            else visit0.recommendation = null;

            bool result = dB.UpdateVisit(visit0, procedures, treatment);

            if (result == true)
            {
                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK);
                this.DialogResult = true;
                this.Close();
            }

        }

        private void ButtonClickPrint(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
