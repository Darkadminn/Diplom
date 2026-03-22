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
    /// Логика взаимодействия для AdminAddUpdatePatient.xaml
    /// </summary>
    public partial class AdminAddUpdatePatient : Window
    {
        DB dB = new DB();
        Patient patient0 = new Patient();
        List<Wing> wings = new List<Wing>();
        List<Individual> individuals = new List<Individual>();
        List<Children> childrens = new List<Children>();
        public AdminAddUpdatePatient()
        {
            InitializeComponent();

            ButtonOperation.Content = "Добавить";
            ButtonOperation.Click += ButtonClickAdd;

            NewIndividualCheckBox.Visibility = Visibility.Visible;
            NewIndividualCheckBox.Checked += NewIndividualCheckBox_Checked;
            NewIndividualCheckBox.Unchecked += NewIndividualCheckBox_Unchecked;

            IndividualStackPanel.Visibility = Visibility.Collapsed;

            wings = dB.GetPoliclinicWings();
            WingPolyclinic.ItemsSource = wings;

            individuals = dB.GetIndividuals();
            Individuals.ItemsSource = individuals;

            childrens = dB.GetChildrens();
            Childrens.ItemsSource = childrens;
        }

        public AdminAddUpdatePatient(Patient patient)
        {
            InitializeComponent();

            ButtonOperation.Content = "Сохранить";
            ButtonOperation.Click += ButtonClickUpdate;

            NewIndividualCheckBox.Visibility = Visibility.Collapsed;

            IndividualStackPanel.Visibility = Visibility.Visible;

            patient0 = patient;

            wings = dB.GetPoliclinicWings();
            WingPolyclinic.ItemsSource = wings;

            individuals = dB.GetIndividuals();
            Individuals.ItemsSource = individuals;

            childrens = dB.GetChildrens();
            Childrens.ItemsSource = childrens;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private bool IsTextAllowed(string text)
        {
            // Проверяем каждый символ
            foreach (char c in text)
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        private void LastName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void FirstName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void MiddleName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
           
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            
        }

        private void NewIndividualCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IndividualStackPanel.Visibility = Visibility.Visible;

            Individuals.IsEnabled = false;
            Individuals.SelectedIndex = -1;
        }

        private void NewIndividualCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IndividualStackPanel.Visibility = Visibility.Collapsed;

            if (WingPolyclinic.SelectedItem != null) Individuals.IsEnabled = true;
            else
            {
                Individuals.IsEnabled = false;
                Individuals.SelectedIndex = -1;
            }
        }

        private void ButtonClickAddChildren(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickUpdateChildren(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClickDeleteChildren(object sender, RoutedEventArgs e)
        {

        }
    }
}
