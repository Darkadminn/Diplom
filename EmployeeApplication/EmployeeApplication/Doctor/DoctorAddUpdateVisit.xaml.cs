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
        List<MedicalProcedure> medicalProcedures = new List<MedicalProcedure>();
        public DoctorAddUpdateVisit(Visit visit, bool read)
        {
            InitializeComponent();

            DateVisit.SelectedDate = visit.date.Date;
            TimeVisit.Text = visit.date.ToString("hh:mm");
            Patient.Text = visit.patient;

            medicalProcedures = dB.GetMedicalProcedures();
            Procedure.ItemsSource = medicalProcedures;

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

                DataGridProcedures.ItemsSource = procedures;

                Subjective.IsEnabled = false;
                Objective.IsEnabled = false;
                Diagnosis.IsEnabled = false;
                Recommendation.IsEnabled = false;
                Procedure.IsEnabled = false;
                Count.IsEnabled = false;
                Comment.IsEnabled = false;

                ButtonAdd.IsEnabled = false;
                ButtonUpdate.IsEnabled = false;
                ButtonSave.IsEnabled = false;
                ButtonSave.Visibility = Visibility.Hidden;

                DataGridProcedures.IsEnabled = false;
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

        private void DataGridProcedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataGridProcedures.SelectedItem != null)
            {
                var procedure = DataGridProcedures.SelectedItem as VisitProcedure;

                Procedure.SelectedItem = medicalProcedures.FirstOrDefault(mp => mp.id == procedure.medicalProcedureId);
                Count.Text = procedure.count.ToString();
                Comment.Text = procedure.comment;
            }
        }

        private void ButtonClickAddProcedure(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickUpdateProcedure(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickDeleteProcedure(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickPrint(object sender, RoutedEventArgs e)
        {

        }
    }
}
