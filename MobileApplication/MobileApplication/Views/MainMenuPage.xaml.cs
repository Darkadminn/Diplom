namespace MobileApplication;

public partial class MainMenuPage : ContentPage
{
	public MainMenuPage()
	{
		InitializeComponent();
	}

    private void ButtonClickedRecord(object sender, EventArgs e)
    {
		Navigation.PushAsync(new RecordPage());
    }

    private void ButtonClickedVisits(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VisitsPage());
    }
}