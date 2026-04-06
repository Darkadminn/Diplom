using System;
using System.Collections.Generic;
using System.IO;
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
        Individual individual0 = new Individual();
        List<Wing> wings = new List<Wing>();
        List<Individual> individuals = new List<Individual>();
        List<Children> childrens = new List<Children>();
        List<Children> childrensIndividual = new List<Children>();
        public AdminAddUpdatePatient()
        {
            InitializeComponent();

            ButtonOperation.Content = "Добавить";
            ButtonOperation.Click += ButtonClickAdd;

            TextBlockHeader.Text = "Добавление пациента";
            this.Title = "ИС «МедСервис» - Добавление пациента";

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

            DataGridChildrens.ItemsSource = childrensIndividual;
        }

        public AdminAddUpdatePatient(Patient patient)
        {
            InitializeComponent();

            ButtonOperation.Content = "Сохранить";
            ButtonOperation.Click += ButtonClickUpdate;

            TextBlockHeader.Text = "Редактирование пациента";
            this.Title = "ИС «МедСервис» - Редактирование пациента";

            NewIndividualCheckBox.Visibility = Visibility.Collapsed;
            Individuals.Visibility = Visibility.Collapsed;
            IndividualsHeader.Visibility = Visibility.Collapsed;

            IndividualStackPanel.Visibility = Visibility.Visible;

            patient0 = patient;

            wings = dB.GetPoliclinicWings();
            WingPolyclinic.ItemsSource = wings;
            WingPolyclinic.SelectedItem = wings.FirstOrDefault(w => w.id == patient.wingId);

            individuals = dB.GetIndividuals();
            Individuals.ItemsSource = individuals;
            Individuals.SelectedItem = individuals.FirstOrDefault(ind => ind.id == patient.individualId);

            childrens = dB.GetChildrens();
            Childrens.ItemsSource = childrens;

            childrensIndividual = dB.GetChildrensIndividual(patient.individualId);
            DataGridChildrens.ItemsSource = childrensIndividual;

            individual0 = Individuals.SelectedItem as Individual;

            LastName.Text = individual0.lastName;
            FirstName.Text = individual0.firstName;
            MiddleName.Text = individual0.middleName;
            InsurancePolicy.Text = individual0.insurancePolicy;
            InsuranceCompany.Text = individual0.insuranceCompany;
            Birthday.SelectedDate = individual0.birthday;
            Phone.Text = individual0.phone;
            birthCertificate.Text = individual0.birthCertificate;
            Snils.Text = individual0.snils;
            Series.Text = individual0.passportSeries;
            Number.Text = individual0.passportNumber;
            IssuedBy.Text = individual0.passportIssuedBy;
            DateIssue.SelectedDate = individual0.passportIssuedDate;

            if(individual0.gender == "Мужской")
            {
                Gender.SelectedIndex = 0;
            }
            else if (individual0.gender == "Женский")
            {
                Gender.SelectedIndex = 1;
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
            if(NewIndividualCheckBox.IsChecked == true)
            {
                if (WingPolyclinic.SelectedItem == null || string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text)
                    || Birthday.SelectedDate == null || Gender.SelectedIndex == -1 || birthCertificate.Text.Contains(birthCertificate.PromptChar.ToString()))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (Birthday.SelectedDate > DateTime.Now || (DateIssue.SelectedDate > DateTime.Now && DateIssue.SelectedDate != null))
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string middleName0;
                    string gender0;
                    var wing = WingPolyclinic.SelectedItem as Wing;

                    if (string.IsNullOrWhiteSpace(MiddleName.Text)) middleName0 = null;
                    else middleName0 = MiddleName.Text;

                    if (Gender.SelectedIndex == 0) gender0 = "Мужской";
                    else gender0 = "Женский";

                    Individual individual = new Individual
                    {
                        lastName = LastName.Text,
                        firstName = FirstName.Text,
                        middleName = middleName0,
                        phone = Phone.Text == "" ? null : Phone.Text,
                        birthday = (DateTime)Birthday.SelectedDate,
                        snils = Snils.Text.Replace("-", "").Replace(" ", "") == "" ? null : Snils.Text,
                        insurancePolicy = InsurancePolicy.Text.Replace(" ", "") == "" ? null : InsurancePolicy.Text.Replace(" ", ""),
                        insuranceCompany = InsuranceCompany.Text == "" ? null : InsuranceCompany.Text,
                        birthCertificate = birthCertificate.Text.Replace(" ",""),
                        gender = gender0
                    };

                    Patient pacient = new Patient
                    {
                        wingId = wing.id
                    };

                    if (!Series.Text.Contains(Series.PromptChar.ToString()) && !Number.Text.Contains(Number.PromptChar.ToString())
                        && !string.IsNullOrWhiteSpace(IssuedBy.Text) && Birthday.SelectedDate != null)
                    {
                        individual.passportSeries = Series.Text;
                        individual.passportNumber = Number.Text;
                        individual.passportIssuedDate = (DateTime)DateIssue.SelectedDate;
                        individual.passportIssuedBy = IssuedBy.Text;
                    }
                    else
                    {
                        individual.passportSeries = null;
                        individual.passportNumber = null;
                        individual.passportIssuedDate = DateTime.Now;
                        individual.passportIssuedBy = null;
                    }



                        bool result = dB.AddPatient(pacient, individual, childrensIndividual, true);

                    if (result == true)
                    {
                        MessageBox.Show("Пациент успешно добавлен", "Успех", MessageBoxButton.OK);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            else
            {
                if (WingPolyclinic.SelectedItem == null || Individuals.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    var individual = Individuals.SelectedItem as Individual;
                    var wing = WingPolyclinic.SelectedItem as Wing;


                    Patient pacient = new Patient
                    {
                        wingId = wing.id,
                        individualId = individual.id,
                    };

                    bool result = dB.AddPatient(pacient, null, childrensIndividual, false);

                    if (result == true)
                    {
                        MessageBox.Show("Пациент успешно добавлен", "Успех", MessageBoxButton.OK);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            if (WingPolyclinic.SelectedItem == null || string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text)
                    || Birthday.SelectedDate == null || Gender.SelectedIndex == -1 || birthCertificate.Text.Contains(birthCertificate.PromptChar.ToString()))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (Birthday.SelectedDate > DateTime.Now || (DateIssue.SelectedDate > DateTime.Now && DateIssue.SelectedDate != null))
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string middleName0;
                string gender0;
                var wing = WingPolyclinic.SelectedItem as Wing;

                if (string.IsNullOrWhiteSpace(MiddleName.Text)) middleName0 = null;
                else middleName0 = MiddleName.Text;

                if (Gender.SelectedIndex == 0) gender0 = "Мужской";
                else gender0 = "Женский";

                Individual individual = new Individual
                {
                    lastName = LastName.Text,
                    firstName = FirstName.Text,
                    middleName = middleName0,
                    phone = Phone.Text == "" ? null : Phone.Text,
                    birthday = (DateTime)Birthday.SelectedDate,
                    snils = Snils.Text.Replace("-", "").Replace(" ", "") == "" ? null : Snils.Text,
                    insurancePolicy = InsurancePolicy.Text.Replace(" ", "") == "" ? null : InsurancePolicy.Text.Replace(" ", ""),
                    insuranceCompany = InsuranceCompany.Text == "" ? null : InsuranceCompany.Text,
                    birthCertificate = birthCertificate.Text.Replace(" ", ""),
                    gender = gender0
                };

                Patient pacient = new Patient
                {
                    wingId = wing.id,
                    individualId = individual0.id,
                    id = patient0.id
                };

                if (!Series.Text.Contains(Series.PromptChar.ToString()) && !Number.Text.Contains(Number.PromptChar.ToString())
                        && !string.IsNullOrWhiteSpace(IssuedBy.Text) && Birthday.SelectedDate != null)
                {
                    individual.passportSeries = Series.Text;
                    individual.passportNumber = Number.Text;
                    individual.passportIssuedDate = (DateTime)DateIssue.SelectedDate;
                    individual.passportIssuedBy = IssuedBy.Text;
                }
                else
                {
                    individual.passportSeries = null;
                    individual.passportNumber = null;
                    individual.passportIssuedDate = DateTime.Now;
                    individual.passportIssuedBy = null;
                }

                bool result = dB.UpdatePatient(pacient, individual, childrensIndividual);

                if (result == true)
                {
                    MessageBox.Show("Данные о пациенте успешно обновлены", "Успех", MessageBoxButton.OK);
                    this.DialogResult = true;
                }
            }
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

            Individuals.IsEnabled = true;

        }

        private void ButtonClickAddChildren(object sender, RoutedEventArgs e)
        {
            if(Childrens.SelectedItem != null)
            {
                var children = Childrens.SelectedItem as Children;
                childrensIndividual.Add(children);
                DataGridChildrens.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClickUpdateChildren(object sender, RoutedEventArgs e)
        {
            if (Childrens.SelectedItem != null && DataGridChildrens.SelectedItem != null)
            {
                var children = Childrens.SelectedItem as Children;
                var childrenDelete = DataGridChildrens.SelectedItem as Children;
                int index = DataGridChildrens.SelectedIndex;
                childrensIndividual.Remove(childrenDelete);
                childrensIndividual.Insert(index, children);
                DataGridChildrens.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClickDeleteChildren(object sender, RoutedEventArgs e)
        {
            if (DataGridChildrens.SelectedItem != null)
            {
                var children = DataGridChildrens.SelectedItem as Children;
                childrensIndividual.Remove(children);
                DataGridChildrens.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGridChildrensSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridChildrens.SelectedItem != null)
            {
                var children = DataGridChildrens.SelectedItem as Children;
                Childrens.SelectedItem = childrens.FirstOrDefault(c => c.id == children.id);
            }

        }
    }
}
