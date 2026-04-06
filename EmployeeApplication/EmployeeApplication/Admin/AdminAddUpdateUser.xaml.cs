using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для AdminAddUpdateUser.xaml
    /// </summary>
    public partial class AdminAddUpdateUser : Window
    {
        DB dB = new DB();
        User user0 = new User();
        List<User> users = new List<User>();
        List<User> filterUsers = new List<User>();
        public AdminAddUpdateUser()
        {
            InitializeComponent();

            ButtonOperation.Content = "Добавить";
            ButtonOperation.Click += ButtonClickAdd;

            TextBlockHeader.Text = "Добавление пользователя";
            this.Title = "ИС «МедСервис» - Добавление пользователя";

            users = dB.GetNoUsers();
            filterUsers = users;
            Users.ItemsSource = filterUsers;
        }

        public AdminAddUpdateUser(User user)
        {
            InitializeComponent();

            ButtonOperation.Content = "Сохранить";
            ButtonOperation.Click += ButtonClickUpdate;

            TextBlockHeader.Text = "Редактирование пользователя";
            this.Title = "ИС «МедСервис» - Редактирование пользователя";

            users = dB.GetUsers();
            filterUsers = users;
            Users.ItemsSource = filterUsers;

            Users.SelectedItem = filterUsers.FirstOrDefault(u => u.id == user.id && u.userId == user.userId && u.post == user.post);

            if(user.role == "Пациент")
            {
                Roles.SelectedIndex = 0;
            }
            else if(user.role == "Администратор")
            {
                Roles.SelectedIndex = 1;
            }
            else if(user.role == "Специалист")
            {
                Roles.SelectedIndex = 2;
            }
            else if(user.role == "Медсестра")
            {
                Roles.SelectedIndex = 3;
            }


            Login.Text = user.login;
            Password.Text = user.password;

            Roles.IsEnabled = false;
            Users.IsEnabled = false;

            user0 = user;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Roles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Roles.SelectedIndex != -1)
            {
                if(Roles.SelectedIndex == 0)
                {
                    filterUsers = users.Where(u => u.role == "Пациент").ToList();
                    Users.ItemsSource = filterUsers;
                    Users.Items.Refresh();
                }
                else if(Roles.SelectedIndex == 1)
                {
                    filterUsers = users.Where(u => u.post == "Администратор").ToList();
                    Users.ItemsSource = filterUsers;
                    Users.Items.Refresh();
                }
                else if(Roles.SelectedIndex == 2)
                {
                    filterUsers = users.Where(u => u.postType == "Врачебный").ToList();
                    Users.ItemsSource = filterUsers;
                    Users.Items.Refresh();
                }
                else if(Roles.SelectedIndex == 3)
                {
                    filterUsers = users.Where(u => u.post == "Медсестра процедурная").ToList();
                    Users.ItemsSource = filterUsers;
                    Users.Items.Refresh();
                }
            }
        }

        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
            
            if (Roles.SelectedIndex == -1 || Users.SelectedItem == null || string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string role;
                var noUser = Users.SelectedItem as User;

                if (Roles.SelectedIndex == 0) role = "Пациент";
                else if (Roles.SelectedIndex == 1) role = "Администратор";
                else if (Roles.SelectedIndex == 2) role = "Специалист";
                else if (Roles.SelectedIndex == 3) role = "Медсестра";
                else role = "";

                User user = new User()
                {
                    id = noUser.id,
                    role = role,
                    login = Login.Text,
                    password = Password.Text,
                };

                bool result = dB.AddUser(user);

                if (result == true)
                {
                    MessageBox.Show("Пациент успешно добавлен", "Успех", MessageBoxButton.OK);
                    this.DialogResult = true;
                    this.Close();
                }
            }
                   
        }

        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                User user = new User()
                {
                    userId = user0.userId,
                    role = user0.role,
                    login = Login.Text,
                    password = Password.Text,
                };

                bool result = dB.UpdateUser(user);

                if (result == true)
                {
                    MessageBox.Show("Данные о пользователе успешно обновлены", "Успех", MessageBoxButton.OK);
                    this.DialogResult = true;
                }
            }
        }
    }
}
