namespace MobileApplication;

public partial class RecordPage : ContentPage
{
	public RecordPage()
	{
		InitializeComponent();

        this.Title = UserAuthorization.fio;
    }

    private void ButtonClickedI(object sender, EventArgs e)
    {
		ReceptionSetting.patientId = UserAuthorization.id;
		ReceptionSetting.patientGender = UserAuthorization.gender;
		ReceptionSetting.patientPolyclinicId = UserAuthorization.polyclinicId;

        Navigation.PushAsync(new TypeWingPage());
    }

    private void ButtonClickedChildren(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ChildrenPage());
    }
}