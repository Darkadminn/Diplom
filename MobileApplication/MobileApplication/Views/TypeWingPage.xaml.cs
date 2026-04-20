namespace MobileApplication;

public partial class TypeWingPage : ContentPage
{
	public TypeWingPage()
	{
		InitializeComponent();

        this.Title = UserAuthorization.fio;
    }

    private void ButtonClickedPolyclinic(object sender, EventArgs e)
    {
        ReceptionSetting.typeWing = "Амбулатория";

        Navigation.PushAsync(new PostDoctorPage());
    }

    private void ButtonClickedHospital(object sender, EventArgs e)
    {
        ReceptionSetting.typeWing = "Стационар";

        Navigation.PushAsync(new PostDoctorPage());
    }
}