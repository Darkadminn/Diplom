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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DB dB = new DB();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClickAuthorization(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Login.Text) || string.IsNullOrEmpty(Password.Password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dB.GetUserAuthorization(Login.Text, Password.Password);

            if (UserAuthorization.id != -1)
            {
                MessageBox.Show($"Добро пожаловать {UserAuthorization.fio}", "Успех", MessageBoxButton.OK);

                if (UserAuthorization.role == "Специалист")
                {
                    var window = new DoctorWindow();
                    window.Show();
                    this.Close();
                }
                else if (UserAuthorization.role == "Администратор")
                {
                    var window = new AdminWindow();
                    window.Show();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
