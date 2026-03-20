
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Net.NetworkInformation;

namespace MobileApplication;

public partial class DoctorsPage : ContentPage
{
    DB dB = new DB();
    List<Filter> filters = new List<Filter>();
    TimeSlot timeSlot = new TimeSlot();
    bool updateFilter;
    Filter userFilter;
    DateTime selectedDate;
    public DoctorsPage()
	{
		InitializeComponent();

        userFilter = new Filter
        {
            name = "000",
            type = "000"
        };

        timeSlot = null;
        updateFilter = false;
        filters = dB.GetFilters();

        string month1 = DateTime.Today.ToString("MMMM");
        string month2 = DateTime.Today.AddDays(14).ToString("MMMM");

        if (month1 != month2)
        {
            MonthYear.Text = $"{char.ToUpper(month1[0])}{month1.Substring(1)}-{char.ToUpper(month2[0])}{month2.Substring(1)}";
        }
        else
        {
            MonthYear.Text = $"{char.ToUpper(month1[0])}{month1.Substring(1)}";
        }

        GenerateCalendar();

    }

    private void GenerateCalendar()
    {
        // Очищаем календарь
        CalendarLayout.Children.Clear();

        // Добавляем дни недели
        AddWeekDaysHeader();

        // Начинаем с текущего дня
        var startDate = DateTime.Today;

        // Заканчиваем через 2 недели
        var endDate = DateTime.Today.AddDays(14);

        var currentDate = startDate;
        Grid currentWeekGrid = null;

        while (currentDate < endDate)
        {
            // Начинаем новую неделю в понедельник
            if (currentDate.DayOfWeek == DayOfWeek.Monday || currentWeekGrid == null)
            {
                currentWeekGrid = CreateWeekGrid();
                CalendarLayout.Children.Add(currentWeekGrid);
            }

            // Получаем правильный индекс столбца для дня недели
            int columnIndex = GetColumnIndex(currentDate.DayOfWeek);
            AddDayToCalendar(currentWeekGrid, currentDate, columnIndex);
            currentDate = currentDate.AddDays(1);
        }
    }

    private Grid CreateWeekGrid()
    {
        return new Grid
        {
            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
            HeightRequest = 50,
            Padding = new Thickness(5, 0)
        };
    }

    private int GetColumnIndex(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            DayOfWeek.Sunday => 6,
            _ => 0
        };
    }

    private void AddWeekDaysHeader()
    {
        var weekDaysGrid = new Grid
        {
            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
            BackgroundColor = Colors.White,
            HeightRequest = 40
        };

        string[] daysOfWeek = { "ПН", "ВТ", "СР", "ЧТ", "ПТ", "Сб", "ВС" };

        for (int i = 0; i < daysOfWeek.Length; i++)
        {
            var dayLabel = new Label
            {
                Text = daysOfWeek[i],
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
            };
            weekDaysGrid.Add(dayLabel, i, 0);
        }

        CalendarLayout.Children.Add(weekDaysGrid);
    }

    private void AddDayToCalendar(Grid weekGrid, DateTime date, int column)
    {
        var isAvailable = IsDateAvailable(date);
        var isSelected = date.Date == selectedDate.Date;

        var dayBorder = new Border
        {
            BackgroundColor = GetBackgroundColor(isSelected, isAvailable),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0
        };

        var dayLabel = new Label
        {
            Text = date.Day.ToString(),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 14,
            TextColor = GetTextColor(isSelected, isAvailable),
            FontAttributes = GetFontAttributes(isSelected)
        };

        dayBorder.Content = dayLabel;

        if (isAvailable)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnDateTapped(date, dayBorder, dayLabel);
            dayBorder.GestureRecognizers.Add(tapGesture);
        }

        weekGrid.Add(dayBorder, column, 0);
    }

    private Color GetBackgroundColor(bool isSelected, bool isAvailable)
    {
        if (!isAvailable)
            return Colors.Transparent;

        if (isSelected)
            return Color.FromArgb("#A8E75D");

        return Color.FromArgb("#99ff99");
    }

    private Color GetTextColor(bool isSelected, bool isAvailable)
    {
        if (!isAvailable)
            return Color.FromArgb("#cccccc");

        if (isSelected)
            return Colors.Black;

        return Colors.Black;
    }

    private FontAttributes GetFontAttributes(bool isSelected)
    {
        if (isSelected)
            return FontAttributes.Bold;

        return FontAttributes.None;
    }

    private bool IsDateAvailable(DateTime date)
    {
        var today = DateTime.Today;
        var maxAvailableDate = today.AddDays(14);
        if (date >= today && date <= maxAvailableDate) return dB.AvailableDay(date, userFilter);
        else return false;
    }

    private void OnDateTapped(DateTime date, Border selectedBorder, Label selectedLabel)
    {
        UpdateCalendarSelection(date);

        selectedDate = date;

        GetListDoctors(date);
    }

    private void UpdateCalendarSelection(DateTime selectedDate)
    {
        foreach (var child in CalendarLayout.Children)
        {
            if (child is Grid weekGrid)
            {
                foreach (var weekChild in weekGrid.Children)
                {
                    if (weekChild is Border dayBorder)
                    {
                        var dayLabel = dayBorder.Content as Label;
                        if (dayLabel != null && int.TryParse(dayLabel.Text, out int dayNumber))
                        {
                            // Ищем дату в диапазоне
                            var today = DateTime.Today;
                            var searchDate = today;
                            var found = false;

                            for (int i = 0; i < 14; i++)
                            {
                                if (searchDate.Day == dayNumber && searchDate.Month == selectedDate.Month)
                                {
                                    var isThisDateSelected = searchDate.Date == selectedDate.Date;
                                    var isAvailable = IsDateAvailable(searchDate);

                                    dayBorder.BackgroundColor = GetBackgroundColor(isThisDateSelected, isAvailable);
                                    dayLabel.TextColor = GetTextColor(isThisDateSelected, isAvailable);
                                    dayLabel.FontAttributes = GetFontAttributes(isThisDateSelected);

                                    found = true;
                                    break;
                                }
                                searchDate = searchDate.AddDays(1);
                            }

                            if (!found)
                            {
                                dayBorder.BackgroundColor = Colors.Transparent;
                                dayLabel.TextColor = Color.FromArgb("#cccccc");
                                dayLabel.FontAttributes = FontAttributes.None;
                            }
                        }
                    }
                }
            }
        }
    }

    private void EntryFilterChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryFilter.Text))
        {
            ListViewFilter.IsVisible = false;
        }
        else
        {
            ListViewFilter.ItemsSource = filters.Where(fil => fil.name.Contains(EntryFilter.Text));
            ListViewFilter.IsVisible = true;

            if (updateFilter == true)
            {
                updateFilter = false;
            }
            else
            {
                userFilter = new Filter
                {
                    name = "000",
                    type = "000"
                };
            }
        }

    }

    private void ListViewFilterItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (ListViewFilter.SelectedItem != null)
        {
            userFilter = (Filter)ListViewFilter.SelectedItem;
            ListViewFilter.IsVisible = false;
            updateFilter = true;
            EntryFilter.Text = userFilter.name;
            UpdateCalendarSelection(DateTime.Today.AddDays(-5));
            selectedDate = DateTime.Today.AddDays(-5);
            ListDoctorsLayout.Children.Clear();
            GenerateCalendar();
        }
        else
        {
            userFilter = new Filter
            {
                name = "000",
                type = "000"
            };

            ListViewFilter.IsVisible = false;
            updateFilter = false;
            EntryFilter.Text = "";
            UpdateCalendarSelection(DateTime.Today.AddDays(-5));
            selectedDate = DateTime.Today.AddDays(-5);
            ListDoctorsLayout.Children.Clear();
            GenerateCalendar();
        }
    }

    private void GetListDoctors(DateTime date)
    {
        ListDoctorsLayout.Children.Clear();

        List<Doctor> doctors = new List<Doctor>();

        doctors = dB.GetDoctors();

        if (userFilter.type == "Врач") doctors = doctors.Where(emp => emp.fio == userFilter.name).ToList();
        else if (userFilter.type == "Город") doctors = doctors.Where(emp => emp.city == userFilter.name).ToList();
        else if (userFilter.type == "Медицинское учреждение") doctors = doctors.Where(emp => emp.wingName == userFilter.name).ToList();

        List<Wing> wings = dB.GetWings().Where(w => doctors.Select(d => d.wingId).Distinct().Contains(w.id)).ToList();

        if (!doctors.Any())
        {
            ListDoctorsLayout.Children.Add(new Label
            {
                Text = "Врачи не найдены",
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20)
            });
            return;
        }

        foreach (var wing in wings)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Start,
                Background = Colors.White,
                Margin = new Thickness(0, 40, 0, 0)
            };

            var labelWing = new Label
            {
                Text = wing.info,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 0, 0, 50)
            };

            var doctorWing = doctors.Where(d => d.wingId == wing.id).ToList();

            stackLayout.Children.Add(labelWing);

            foreach (var doctor in doctorWing)
            {
                List<TimeSlot> slots = dB.AvailableTimeSlot(date, doctor);

                if (slots.Count == 0) continue;

                var stackDoctor = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.Start,
                };

                var stackDoctorInfo = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                };

                var labelDoctor = new Label
                {
                    Text = doctor.fio,
                    TextColor = Colors.Red,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 30,
                    VerticalTextAlignment = TextAlignment.Center
                };

                var buttonInfo = new Button
                {
                    WidthRequest = 30,
                    HeightRequest = 30,
                    Text = "\u2304",
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    Margin = new Thickness(0, 0, 0, 0),
                };

                var stackDoctorVisit = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(10, 0, 0, 0),
                    IsVisible = false
                };

                buttonInfo.Clicked += (sender, e) =>
                {
                    if (stackDoctorVisit.IsVisible == false)
                    {
                        stackDoctorVisit.IsVisible = true;
                        buttonInfo.Text = "^";
                        SlotsDoctor(stackDoctorVisit, date, doctor);
                    }
                    else
                    {
                        stackDoctorVisit.IsVisible = false;
                        buttonInfo.Text = "\u2304";
                        stackDoctorVisit.Children.Clear();
                    }
                };

                stackDoctorInfo.Children.Add(buttonInfo);
                stackDoctorInfo.Children.Add(labelDoctor);

                stackDoctor.Children.Add(stackDoctorInfo);
                stackDoctor.Children.Add(stackDoctorVisit);

                stackLayout.Children.Add(stackDoctor);

            }

            ListDoctorsLayout.Children.Add(stackLayout);
        }

    }

    private async void SlotsDoctor(StackLayout stackDoctorVisit, DateTime date, Doctor doctor)
    {
        var flexSlots = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.Start,
            AlignItems = FlexAlignItems.Start,
            AlignContent = FlexAlignContent.Start
        };

        var buttonDoctor = new Button
        {
            HeightRequest = 30,
            WidthRequest = 200,
            Text = "Записаться",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#cccccc"),
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 16,
            CornerRadius = 5,
            IsEnabled = false,
            Margin = new Thickness(0, 10, 0, 0)
        };

        buttonDoctor.Clicked += async (sender, e) =>
        {
            bool answer = await DisplayAlert("Подтверждение записи", $"Вы точно хотите записаться к этому врачу на {timeSlot.date.ToString("HH:mm")}?", "Да", "Нет");

            if (answer == true)
            {
                dB.InsertVisit(timeSlot.date, this, doctor);

                stackDoctorVisit.IsVisible = false;
                stackDoctorVisit.Children.Clear();
            }
        };

        List<TimeSlot> slots = dB.AvailableTimeSlot(date, doctor);

        foreach (TimeSlot slot in slots)
        {
            var borderSlot = new Border
            {
                BackgroundColor = Color.FromArgb("#00ff7f"),
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 70,
                HeightRequest = 35,
                Margin = new Thickness(0, 0, 10, 10),
                Padding = 0
            };
            var labelSlot = new Label
            {
                WidthRequest = 70,
                HeightRequest = 35,
                Text = slot.date.ToString("HH:mm"),
                TextColor = Colors.Black,
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };


            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                timeSlot = slot;
                UpdateSlots(flexSlots, buttonDoctor);
            };
            borderSlot.GestureRecognizers.Add(tapGesture);

            borderSlot.Content = labelSlot;

            flexSlots.Children.Add(borderSlot);

        }

        stackDoctorVisit.Children.Add(flexSlots);
        stackDoctorVisit.Children.Add(buttonDoctor);
    }

    private async void UpdateSlots(FlexLayout flexSlots, Button buttonDoctor)
    {
        if (timeSlot != null)
        {
            foreach (var border in flexSlots.Children.OfType<Border>())
            {
                if (border.Content is Label label)
                {
                    if (label.Text == timeSlot.date.ToString("HH:mm"))
                    {
                        border.BackgroundColor = Color.FromArgb("#A8E75D");
                        label.TextColor = Colors.Black;

                        buttonDoctor.IsEnabled = true;
                        buttonDoctor.BackgroundColor = Colors.Red;
                        buttonDoctor.TextColor = Colors.Black;
                    }
                    else
                    {
                        border.BackgroundColor = Color.FromArgb("#00ff7f");
                        label.TextColor = Colors.Black;
                    }
                }

            }
        }
    }
}