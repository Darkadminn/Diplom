namespace MobileApplication.Views;

public partial class RegistrationPage : ContentPage
{
	DB dB = new DB();
	public RegistrationPage()
	{
		InitializeComponent();
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

    private async void ButtonClickedAddUser(object sender, EventArgs e)
    {
		if(Phone.Text.Replace("(", "").Replace(")", "").Replace("-","").Length != 12 || string.IsNullOrWhiteSpace(Login.Text) 
			|| string.IsNullOrWhiteSpace(Password.Text))
		{
            await DisplayAlert("Ошибка", "Заполните все поля", "ОК");
        }
		else
		{
            if(Password.Text.Length < 8)
            {
                await DisplayAlert("Ошибка", "Длина пароля должна быть не менее 8 символов", "ОК");
                return;
            }

            if(Password.Text != RepeatPassword.Text)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return;
            }

			string result = dB.AvailablePhoneLogin(Phone.Text, Login.Text);

			if(result == "1")
			{
				await Navigation.PushAsync(new PhoneConfirmationPage(Phone.Text, Login.Text, Password.Text));
            }
            else if(result == "-1")
            {
                await DisplayAlert("Ошибка", "Нет пациента с таким номером телефона", "ОК");
            }
			else if(result == "-2")
			{
                await DisplayAlert("Ошибка", "Уже есть пользователь с таким номером телефона", "ОК");
            }
            else if (result == "-3")
            {
                await DisplayAlert("Ошибка", "Уже есть пользователь с таким номером телефона", "ОК");
            }
        }
    }
}