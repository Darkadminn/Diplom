namespace MobileApplication;

public partial class RecoveryPage : ContentPage
{
    DB dB = new DB();
	public RecoveryPage()
	{
		InitializeComponent();
	}

    private async void ButtonClickedNext(object sender, EventArgs e)
    {
        if (Phone.Text.Replace("(", "").Replace(")", "").Replace("-", "").Length != 12)
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "ОК");
        }
        else
        {

            string result = dB.AvailableUserPhone(Phone.Text);

            if (result == "1")
            {
                await Navigation.PushAsync(new PhoneConfirmationPage(Phone.Text));
            }
            else if (result == "-1")
            {
                await DisplayAlert("Ошибка", "Нет пользователя с таким номером телефона", "ОК");
            }
            
        }
    }
}