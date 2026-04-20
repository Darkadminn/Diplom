using MobileApplication.Views;

namespace MobileApplication
{
    public partial class MainPage : ContentPage
    {
        DB dB = new DB();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonClickedAuthorization(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                await DisplayAlert("Ошибка", "Заполните все поля", "ОК");
                return;
            }

            ButtonLogin.IsEnabled = false;

            dB.GetUserAuthorization(Login.Text, Password.Text);

            if(UserAuthorization.id != -1)
            {

                await DisplayAlert("Успех", $"Добро пожаловать, {UserAuthorization.fio}", "ОК");

                await Navigation.PushAsync(new MainMenuPage());
            }
            else
            {
                await DisplayAlert("Ошибка", "Неверный логин или пароль", "ОК");
            }

            ButtonLogin.IsEnabled = true;

        }

        private async void ButtonClickedRegistration(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        private async void RecoveryPasswordTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new RecoveryPage());
        }
    }
}
