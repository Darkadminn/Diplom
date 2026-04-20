namespace MobileApplication;

public partial class PostDoctorPage : ContentPage
{
	DB dB = new DB();
	public PostDoctorPage()
	{
		InitializeComponent();

        this.Title = UserAuthorization.fio;

        ButtonForward.IsEnabled = false;
		ListPosts.ItemsSource = dB.GetPosts();
	}

    private void ListPostsItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
		var post = ListPosts.SelectedItem as Post;
		ReceptionSetting.postId = post.id;

        ButtonForward.IsEnabled = true;
    }

    private void ButtonForwardClickedDoctors(object sender, EventArgs e)
    {
		Navigation.PushAsync(new DoctorsPage());
    }
}