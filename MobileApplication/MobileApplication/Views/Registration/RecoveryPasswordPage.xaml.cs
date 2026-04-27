using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MobileApplication;

public partial class RecoveryPasswordPage : ContentPage
{
	string phone0;
    DB dB = new DB();
	public RecoveryPasswordPage(string phone)
	{
		InitializeComponent();

		phone0 = phone;
	}

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        CheckPasswordsMatch();
    }

    private void OnRepeatPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        CheckPasswordsMatch();
    }

    private void CheckPasswordsMatch()
    {
        if (string.IsNullOrEmpty(RepeatPassword.Text))
        {
            BorderRepeatPassword.StrokeThickness = 1;
            return;
        }

        if (Password.Text != RepeatPassword.Text)
        {
            BorderRepeatPassword.StrokeThickness = 1;
        }
        else
        {
            BorderRepeatPassword.StrokeThickness = 0;
        }
    }

    private async void ButtonClickedUpdateUser(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Password.Text))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "ОК");
        }
        else
        {
            if (Password.Text.Length < 8)
            {
                await DisplayAlert("Ошибка", "Длина пароля должна быть не менее 8 символов", "ОК");
                return;
            }

            if (Password.Text != RepeatPassword.Text)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return;
            }

            string result = dB.UpdateUser(Password.Text, phone0);

            if (result == "1")
            {
                await DisplayAlert("Успех", "Пароль успешно обновлен", "ОК");
                await Navigation.PushAsync(new MainPage());
            }
            
        }
    }
}