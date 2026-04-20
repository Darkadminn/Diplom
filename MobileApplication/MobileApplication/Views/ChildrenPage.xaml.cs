namespace MobileApplication;

public partial class ChildrenPage : ContentPage
{
	DB dB = new DB();
    List<Children> childrens = new List<Children>();

	public ChildrenPage()
	{
		InitializeComponent();

        this.Title = UserAuthorization.fio;

        childrens = dB.GetChildrens();

		PickerChildrens.ItemsSource = childrens;

    }

    private void PickerChildrens_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(PickerChildrens.SelectedIndex != -1)
        {
            ButtonChildren.IsEnabled = true;
        }
        else
        {
            ButtonChildren.IsEnabled = false;
        }
    }

    private void ButtonChildrenClicked(object sender, EventArgs e)
    {
        if(PickerChildrens.SelectedIndex != -1)
        {
            var children = PickerChildrens.SelectedItem as Children;

            ReceptionSetting.patientId = children.id;
            ReceptionSetting.patientGender = children.gender;
            ReceptionSetting.patientPolyclinicId = children.wingId;

            Navigation.PushAsync(new TypeWingPage());
        }
    }

    
}