using Microsoft.Maui.Controls.Shapes;

namespace MobileApplication;

public partial class VisitsPage : ContentPage
{
    DB dB = new DB();
    public VisitsPage()
	{
		InitializeComponent();

        LoadVisits();
	}

    private void LoadVisits()
    {
        CurrentVisits.Children.Clear();
        OldVisits.Children.Clear();

        List<Visit> visits = dB.GetVisits();

        List<Visit> currentVisit = visits.Where(v => v.status == "Запланирован" || v.status == "Перенесен").ToList();

        List<Visit> oldVisits = visits.Where(v => v.status == "Завершен").ToList();

        foreach (Visit visit in currentVisit)
        {
            var mainStackLayet = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                WidthRequest = 300
            };

            var mainBorder = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                BackgroundColor = Color.FromArgb("#bef574"),
                Margin = new Thickness(0, 0, 25, 0)
            };

            var wingLabel = new Label
            {
                Text = $"Корпус: {visit.wingName}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var cabinetLabel = new Label
            {
                Text = $"Кабинет: {visit.cabinet}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var fioLabel = new Label
            {
                Text = $"ФИО врача: {visit.fio}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var postLabel = new Label
            {
                Text = $"Должность: {visit.postName}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var dateLabel = new Label
            {
                Text = $"Дата: {visit.date.ToString("dd:MM:yyyy")}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var timeLabel = new Label
            {
                Text = $"Время: {visit.date.ToString("HH:mm")}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var statusLabel = new Label
            {
                Text = $"Статус: {visit.status}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var deleteButton = new Button
            {
                Text = "Отменить",
                TextColor = Colors.Black,
                BackgroundColor = Colors.Red,
                WidthRequest = 150,
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };


            deleteButton.Clicked += async (sender, e) =>
            {
                bool answer = await DisplayAlert("Подтверждение отмены записи", $"Вы точно хотите отменить эту запись?", "Да", "Нет");

                if (answer == true)
                {
                    dB.DeleteVisit(visit.id);

                    CurrentVisits.Remove(mainBorder);
                }
            };

            mainStackLayet.Children.Add(wingLabel);
            mainStackLayet.Children.Add(cabinetLabel);
            mainStackLayet.Children.Add(fioLabel);
            mainStackLayet.Children.Add(postLabel);
            mainStackLayet.Children.Add(dateLabel);
            mainStackLayet.Children.Add(timeLabel);
            mainStackLayet.Children.Add(statusLabel);
            mainStackLayet.Children.Add(deleteButton);

            mainBorder.Content = mainStackLayet;

            CurrentVisits.Children.Add(mainBorder);
        }


        foreach (Visit visit in oldVisits)
        {
            var mainStackLayet = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                WidthRequest = 300
            };

            var mainBorder = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                BackgroundColor = Color.FromArgb("#bef574"),
                Margin = new Thickness(0, 0, 20, 0)
            };

            var wingLabel = new Label
            {
                Text = $"Корпус: {visit.wingName}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var cabinetLabel = new Label
            {
                Text = $"Кабинет: {visit.cabinet}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var fioLabel = new Label
            {
                Text = $"ФИО врача: {visit.fio}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var postLabel = new Label
            {
                Text = $"Должность: {visit.postName}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var dateLabel = new Label
            {
                Text = $"Дата: {visit.date.ToString("dd:MM:yyyy")}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var timeLabel = new Label
            {
                Text = $"Время: {visit.date.ToString("HH:mm")}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            var statusLabel = new Label
            {
                Text = $"Статус: {visit.status}",
                TextColor = Colors.Black,
                Margin = new Thickness(10, 5, 0, 0),
                FontSize = 12
            };

            mainStackLayet.Children.Add(wingLabel);
            mainStackLayet.Children.Add(cabinetLabel);
            mainStackLayet.Children.Add(fioLabel);
            mainStackLayet.Children.Add(postLabel);
            mainStackLayet.Children.Add(dateLabel);
            mainStackLayet.Children.Add(timeLabel);
            mainStackLayet.Children.Add(statusLabel);

            mainBorder.Content = mainStackLayet;

            OldVisits.Children.Add(mainBorder);
        }

    }
}