namespace MobileApplication;

public partial class ChildrenPage : ContentPage
{
	DB dB = new DB();

	public ChildrenPage()
	{
		InitializeComponent();

        var childrens = dB.GetChildrens();
        childrens.Add(new Children
        {
            id = -5,
            lastName = "Добавить нового ребенка"
        });

		PickerChildrens.ItemsSource = childrens;

        DataChildren.IsVisible = false;

    }

    private void PickerChildrens_SelectedIndexChanged(object sender, EventArgs e)
    {
        var children = PickerChildrens.SelectedItem as Children;

        if(children.id == -5)
        {
            DataChildren.IsVisible = true;
        }
        else
        {
            DataChildren.IsVisible = false;
        }
    }
}