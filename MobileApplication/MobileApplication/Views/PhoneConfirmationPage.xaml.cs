using System;
using Android.Telephony;

namespace MobileApplication;

public partial class PhoneConfirmationPage : ContentPage
{
    string phone0;
    string code0;
    string login0;
    string password0;
    bool insertUser = false;
    Random random0 = new Random();
    DB dB = new DB();
	public PhoneConfirmationPage(string phone)
	{
		InitializeComponent();

        phone0 = phone;

        SendSMS(phone0);
	}

    public PhoneConfirmationPage(string phone, string login, string password)
    {
        InitializeComponent();

        phone0 = phone;

        insertUser = true;

        SendSMS(phone0);
    }

    private string GenerateNumericCode()
    {
        var code = string.Empty;
        for (int i = 0; i < 6; i++)
        {
            code += random0.Next(0, 9).ToString();
        }
        return code;
    }

    private async void SendSMS(string phone)
    {
        try
        {
            code0 = GenerateNumericCode();

            //string[] recipients = new[] { "+7(965)921-75-76" };

            SmsManager smsManager = SmsManager.Default;

            smsManager.SendTextMessage("+7(965)921-75-76", null, $"Код - {code0}", null, null);

            /*var message = new SmsMessage($"Код - {code0}", recipients);

            await Sms.Default.ComposeAsync(message);*/
        }
        catch (Java.Lang.SecurityException ex)
        {
            await DisplayAlert("Ошибка безопасности",
                "Нет разрешения на отправку SMS. Разрешите доступ в настройках приложения.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось отправить SMS: {ex.Message}", "OK");
        }

    }

    private async void ButtonClickedNext(object sender, EventArgs e)
    {
        if(Code.Text == code0)
        {
            if(insertUser)
            {
                string result = dB.InsertUser(login0, password0, phone0);

                if(result == "1")
                {
                    await DisplayAlert("Успех", "Регистрация прошла успешно", "ОК");

                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    await DisplayAlert("Ошибка", "Ошибка во время регистрации", "ОК");
                }
            }
            else
            {
                await Navigation.PushAsync(new RecoveryPasswordPage(phone0));
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный код", "ОК");
        }
    }
}