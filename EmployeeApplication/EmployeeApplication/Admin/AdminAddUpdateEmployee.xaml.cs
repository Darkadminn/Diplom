using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Логика взаимодействия для AdminAddUpdateEmployee.xaml
    /// </summary>
    public partial class AdminAddUpdateEmployee : Window
    {
        DB dB = new DB();
        Employee employee0 = new Employee();
        Individual individual0 = new Individual();
        List<Post> posts = new List<Post>();
        List<Individual> individuals = new List<Individual>();
        List<Children> childrens = new List<Children>();
        List<Children> childrensIndividual = new List<Children>();
        public AdminAddUpdateEmployee()
        {
            InitializeComponent();

            ButtonOperation.Content = "Добавить";
            ButtonOperation.Click += ButtonClickAdd;

            TextBlockHeader.Text = "Добавление сотрудника";
            this.Title = "ИС «МедСервис» - Добавление сотрудника";

            NewIndividualCheckBox.Visibility = Visibility.Visible;
            NewIndividualCheckBox.Checked += NewIndividualCheckBox_Checked;
            NewIndividualCheckBox.Unchecked += NewIndividualCheckBox_Unchecked;

            IndividualStackPanel.Visibility = Visibility.Collapsed;

            posts = dB.GetPosts();
            Posts.ItemsSource = posts;

            individuals = dB.GetIndividuals();
            Individuals.ItemsSource = individuals;

            childrens = dB.GetChildrens();
            Childrens.ItemsSource = childrens;

            DataGridChildrens.ItemsSource = childrensIndividual;
        }

        public AdminAddUpdateEmployee(Employee employee)
        {
            InitializeComponent();

            ButtonOperation.Content = "Сохранить";
            ButtonOperation.Click += ButtonClickUpdate;

            TextBlockHeader.Text = "Редактирование сотрудника";
            this.Title = "ИС «МедСервис» - Редактирование сотрудника";

            NewIndividualCheckBox.Visibility = Visibility.Collapsed;
            Individuals.Visibility = Visibility.Collapsed;
            IndividualsHeader.Visibility = Visibility.Collapsed;

            IndividualStackPanel.Visibility = Visibility.Visible;

            employee0 = employee;

            posts = dB.GetPosts();
            Posts.ItemsSource = posts;
            Posts.SelectedItem = posts.FirstOrDefault(w => w.id == employee.postId);

            individuals = dB.GetIndividuals();
            Individuals.ItemsSource = individuals;
            Individuals.SelectedItem = individuals.FirstOrDefault(ind => ind.id == employee.individualId);

            childrens = dB.GetChildrens();
            Childrens.ItemsSource = childrens;

            childrensIndividual = dB.GetChildrensIndividual(employee.individualId);
            DataGridChildrens.ItemsSource = childrensIndividual;

            DateAdmission.SelectedDate = employee.dateAdmission;
            DateDismissal.SelectedDate = employee.dateDismissal;

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

            if (individual0.gender == "Мужской")
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
            if (NewIndividualCheckBox.IsChecked == true)
            {
                if (Posts.SelectedItem == null || DateAdmission.SelectedDate == null || string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text)
                    || Birthday.SelectedDate == null || Gender.SelectedIndex == -1 || birthCertificate.Text.Contains(birthCertificate.PromptChar.ToString()) 
                    || InsurancePolicy.Text.Contains(InsurancePolicy.PromptChar.ToString()) || string.IsNullOrWhiteSpace(InsuranceCompany.Text)
                    || Snils.Text.Contains(Snils.PromptChar.ToString()) || Phone.Text.Contains(Phone.PromptChar.ToString()) || Series.Text.Contains(Series.PromptChar.ToString())
                    || Number.Text.Contains(Number.PromptChar.ToString()) || string.IsNullOrWhiteSpace(IssuedBy.Text) || DateIssue.SelectedDate == null)
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (Birthday.SelectedDate > DateTime.Now || (DateIssue.SelectedDate > DateTime.Now && DateIssue.SelectedDate != null)
                        || DateAdmission.SelectedDate > DateTime.Now || (DateDismissal.SelectedDate > DateTime.Now && DateDismissal.SelectedDate != null))
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateAdmission.SelectedDate > DateDismissal.SelectedDate && DateDismissal.SelectedDate != null)
                    {
                        MessageBox.Show("Дата приема не может быть больше даты увольнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string middleName0;
                    string gender0;
                    var post = Posts.SelectedItem as Post;

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
                        gender = gender0,
                        passportSeries = Series.Text,
                        passportNumber = Number.Text,
                        passportIssuedDate = (DateTime)DateIssue.SelectedDate,
                        passportIssuedBy = IssuedBy.Text
                    };

                    Employee employee = new Employee
                    {
                        postId = post.id,
                        dateAdmission = (DateTime)DateAdmission.SelectedDate,
                        dateDismissal = DateDismissal.SelectedDate,
                    };



                    bool result = dB.AddEmployee(employee, individual, childrensIndividual, true);

                    if (result == true)
                    {
                        MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            else
            {
                if (Posts.SelectedItem == null || DateAdmission.SelectedDate == null || Individuals.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (DateAdmission.SelectedDate > DateTime.Now || (DateDismissal.SelectedDate > DateTime.Now && DateDismissal.SelectedDate != null))
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateAdmission.SelectedDate > DateDismissal.SelectedDate && DateDismissal.SelectedDate != null)
                    {
                        MessageBox.Show("Дата приема не может быть больше даты увольнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var individual = Individuals.SelectedItem as Individual;
                    var post = Posts.SelectedItem as Post;


                    Employee employee = new Employee
                    {
                        postId = post.id,
                        dateAdmission = (DateTime)DateAdmission.SelectedDate,
                        dateDismissal = DateDismissal.SelectedDate,
                        individualId = individual.id,
                    };

                    bool result = dB.AddEmployee(employee, null, childrensIndividual, false);

                    if (result == true)
                    {
                        MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            if (Posts.SelectedItem == null || DateAdmission.SelectedDate == null || string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text)
                    || Birthday.SelectedDate == null || Gender.SelectedIndex == -1 || birthCertificate.Text.Contains(birthCertificate.PromptChar.ToString())
                    || InsurancePolicy.Text.Contains(InsurancePolicy.PromptChar.ToString()) || string.IsNullOrWhiteSpace(InsuranceCompany.Text)
                    || Snils.Text.Contains(Snils.PromptChar.ToString()) || Phone.Text.Contains(Phone.PromptChar.ToString()) || Series.Text.Contains(Series.PromptChar.ToString())
                    || Number.Text.Contains(Number.PromptChar.ToString()) || string.IsNullOrWhiteSpace(IssuedBy.Text) || DateIssue.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (Birthday.SelectedDate > DateTime.Now || (DateIssue.SelectedDate > DateTime.Now && DateIssue.SelectedDate != null)
                        || DateAdmission.SelectedDate > DateTime.Now || (DateDismissal.SelectedDate > DateTime.Now && DateDismissal.SelectedDate != null))
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateAdmission.SelectedDate > DateDismissal.SelectedDate && DateDismissal.SelectedDate != null)
                {
                    MessageBox.Show("Дата приема не может быть больше даты увольнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string middleName0;
                string gender0;
                var post = Posts.SelectedItem as Post;

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
                    gender = gender0,
                    passportSeries = Series.Text,
                    passportNumber = Number.Text,
                    passportIssuedDate = (DateTime)DateIssue.SelectedDate,
                    passportIssuedBy = IssuedBy.Text
                };

                Employee employee = new Employee
                {
                    postId = post.id,
                    dateAdmission = (DateTime)DateAdmission.SelectedDate,
                    dateDismissal = DateDismissal.SelectedDate,
                    individualId = individual0.id,
                    id = employee0.id
                };

                bool result = dB.UpdateEmployee(employee, individual, childrensIndividual);

                if (result == true)
                {
                    MessageBox.Show("Данные о сотруднике успешно обновлены", "Успех", MessageBoxButton.OK);
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
            if (Childrens.SelectedItem != null)
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
