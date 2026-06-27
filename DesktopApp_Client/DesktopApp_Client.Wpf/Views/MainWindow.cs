using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Activities;
using DesktopApp_Client.Wpf.Models.Statistics;
using DesktopApp_Client.Wpf.Models.Users;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class MainWindow : Window
{
    private readonly StatisticsClientService _statisticsService = new();
    private readonly UserClientService _userService = new();
    private readonly ActivityClientService _activityService = new();

    private readonly ContentControl _contentHost = new();
    private readonly TextBlock _pageTitle = new();
    private readonly Dictionary<string, Button> _navButtons = new();
    private DataGrid? _userGrid;
    private DataGrid? _activityGrid;
    private DataGrid? _approvalGrid;
    private ComboBox? _roleComboBox;
    private readonly ActivityTypeOption[] _activityTypes =
    [
        new(1, "Workshop"),
        new(2, "Seminar"),
        new(3, "Competition"),
        new(4, "Volunteer"),
        new(5, "Club Event")
    ];

    public MainWindow()
    {
        WpfUi.ApplyWindow(this, "UA Admin", 1440, 860);
        Content = BuildShell();
        ShowOverview();
    }

    private Grid BuildShell()
    {
        var root = new Grid { Background = WpfUi.PageBackground };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(260) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var sidebar = new Border
        {
            Background = new LinearGradientBrush(Color.FromRgb(139, 92, 246), Color.FromRgb(216, 180, 254), 90),
            Padding = new Thickness(22, 30, 22, 22)
        };
        var nav = new StackPanel();
        nav.Children.Add(LogoBrand());
        nav.Children.Add(NavButton("Overview", ShowOverview));
        nav.Children.Add(NavButton("User Management", async () => await ShowUsersAsync()));
        nav.Children.Add(NavButton("Activity Management", async () => await ShowActivitiesAsync()));
        nav.Children.Add(NavButton("Activity Approval", async () => await ShowApprovalsAsync()));
        nav.Children.Add(NavButton("Statistics", async () => await ShowStatisticsAsync()));
        sidebar.Child = nav;
        root.Children.Add(sidebar);

        var main = new Grid { Margin = new Thickness(34, 28, 34, 28) };
        main.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        main.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var topbar = new Border
        {
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(22, 16, 22, 16),
            Margin = new Thickness(0, 0, 0, 20)
        };
        var topbarGrid = new Grid();
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _pageTitle.FontWeight = FontWeights.SemiBold;
        _pageTitle.Foreground = WpfUi.Text;
        _pageTitle.VerticalAlignment = VerticalAlignment.Center;
        topbarGrid.Children.Add(_pageTitle);
        var admin = new TextBlock
        {
            Text = TokenStorage.Email ?? "Admin",
            Foreground = WpfUi.Text,
            Margin = new Thickness(0, 0, 16, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(admin, 1);
        topbarGrid.Children.Add(admin);
        var logout = WpfUi.DangerButton("Logout");
        logout.Click += (_, _) => Logout();
        Grid.SetColumn(logout, 2);
        topbarGrid.Children.Add(logout);
        topbar.Child = topbarGrid;
        main.Children.Add(topbar);

        var scroll = new ScrollViewer
        {
            Content = _contentHost,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(scroll, 1);
        main.Children.Add(scroll);
        Grid.SetColumn(main, 1);
        root.Children.Add(main);
        return root;
    }

    private Button NavButton(string text, Action action)
    {
        var button = WpfUi.SidebarButton(text, false, action);
        _navButtons[text] = button;
        return button;
    }

    private static UIElement LogoBrand()
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(6, 0, 0, 30)
        };
        row.Children.Add(new Image
        {
            Source = new BitmapImage(new Uri("Assets/ua-logo.png", UriKind.Relative)),
            Width = 54,
            Height = 54,
            Margin = new Thickness(0, 0, 12, 0)
        });
        row.Children.Add(new TextBlock
        {
            Text = "UA Admin",
            Foreground = Brushes.White,
            FontSize = 25,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center
        });
        return row;
    }

    private void Activate(string title)
    {
        _pageTitle.Text = title;
        foreach (var (key, button) in _navButtons)
        {
            button.Background = key == title
                ? new SolidColorBrush(Color.FromArgb(70, 255, 255, 255))
                : Brushes.Transparent;
        }
    }

    private async void ShowOverview()
    {
        Activate("Overview");
        var content = new StackPanel();

        var stats = await GetStatisticsOrDefaultAsync();
        var cards = new UniformGrid { Columns = 3, Margin = new Thickness(0, 0, 0, 6) };
        cards.Children.Add(WpfUi.MetricCard("Total Activities", stats.TotalActivities.ToString(), WpfUi.Primary, "All activities"));
        cards.Children.Add(WpfUi.MetricCard("Approved Activities", stats.ApprovedActivities.ToString(), new SolidColorBrush(Color.FromRgb(16, 185, 129)), "Approved by staff"));
        cards.Children.Add(WpfUi.MetricCard("Pending Activities", stats.PendingActivities.ToString(), new SolidColorBrush(Color.FromRgb(245, 158, 11)), "Waiting for review"));
        content.Children.Add(cards);

        var lower = new UniformGrid { Columns = 2 };
        lower.Children.Add(PanelCard("Admin Permissions", TaskNotes()));
        lower.Children.Add(PanelCard("Seed Data Summary", QuickStats(stats)));
        content.Children.Add(lower);

        _contentHost.Content = content;
    }

    private static UIElement TaskNotes()
    {
        var panel = new StackPanel();
        panel.Children.Add(Note("Admin can manage all users through /api/Users."));
        panel.Children.Add(Note("Admin can create, edit, delete, approve, and reject activities."));
        panel.Children.Add(Note("Admin can view staff review data and system statistics."));
        return panel;
    }

    private static UIElement QuickStats(AdminStatisticsResponse stats)
    {
        var panel = new StackPanel();
        panel.Children.Add(StatLine("Total Registrations", stats.TotalRegistrations.ToString()));
        panel.Children.Add(StatLine("Participated Students", stats.ParticipatedStudents.ToString()));
        panel.Children.Add(StatLine("API Status", "Connected"));
        return panel;
    }

    private async Task ShowUsersAsync()
    {
        Activate("User Management");
        var root = new StackPanel();
        root.Children.Add(PageHeader("User Management", "Create accounts, change roles, lock or unlock users."));

        var toolbar = new WrapPanel { Margin = new Thickness(0, 0, 0, 12) };
        AddButton(toolbar, "Refresh", async () => await LoadUsersAsync(), false);
        AddButton(toolbar, "Create User", () => { ShowCreateUser(); return Task.CompletedTask; }, true);
        AddButton(toolbar, "View Detail", async () => await ShowSelectedUserDetailAsync(), false);

        _roleComboBox = new ComboBox { Width = 150, ItemsSource = new[] { "Admin", "Staff", "Organizer", "Student" } };
        toolbar.Children.Add(new TextBlock { Text = "Role:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 6, 0) });
        toolbar.Children.Add(_roleComboBox);
        AddButton(toolbar, "Change Role", async () => await ChangeRoleAsync(), false);
        AddButton(toolbar, "Lock", async () => await LockUserAsync(), false);
        AddButton(toolbar, "Unlock", async () => await UnlockUserAsync(), false);
        root.Children.Add(toolbar);

        _userGrid = WpfUi.Grid();
        root.Children.Add(TableCard(_userGrid));
        _contentHost.Content = root;
        await LoadUsersAsync();
    }

    private void ShowCreateUser()
    {
        Activate("User Management");
        var panel = new StackPanel { MaxWidth = 720 };
        panel.Children.Add(PageHeader("Create User", "Fill the form and submit to POST /api/Users."));
        var fullName = AddText(panel, "Full Name");
        var email = AddText(panel, "Email");
        var password = AddPassword(panel, "Password");
        var dob = new DatePicker { SelectedDate = DateTime.Today.AddYears(-20) };
        AddControl(panel, "Date of Birth", dob);
        var studentCode = AddText(panel, "Student Code");
        var department = AddText(panel, "Department");
        var role = new ComboBox { ItemsSource = new[] { "Admin", "Staff", "Organizer", "Student" } };
        AddControl(panel, "Role", role);

        var actions = new WrapPanel { Margin = new Thickness(0, 8, 0, 0) };
        AddButton(actions, "Back to Users", async () => await ShowUsersAsync(), false);
        AddButton(actions, "Create", async () =>
        {
            var validation = ValidateCreate(fullName.Text, email.Text, password.Password, dob.SelectedDate, department.Text, role.SelectedItem);
            if (validation is not null)
            {
                MessageBox.Show(validation, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = await _userService.CreateUserAsync(new CreateUserRequest
            {
                FullName = fullName.Text.Trim(),
                Email = email.Text.Trim(),
                Password = password.Password,
                DateOfBirth = dob.SelectedDate!.Value.Date,
                StudentCode = string.IsNullOrWhiteSpace(studentCode.Text) ? null : studentCode.Text.Trim(),
                Department = department.Text.Trim(),
                Role = role.SelectedItem!.ToString()!
            });
            ShowResult(result.Success, result.Message);
            if (result.Success)
            {
                await ShowUsersAsync();
            }
        }, true);
        panel.Children.Add(actions);
        _contentHost.Content = WpfUi.Card(panel);
    }

    private async Task ShowSelectedUserDetailAsync()
    {
        if (_userGrid?.SelectedItem is not UserGridRow row)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.GetUserAsync(row.Id);
        if (!result.Success || result.Data is null)
        {
            ShowResult(false, result.Message);
            return;
        }

        var user = result.Data;
        var panel = new StackPanel();
        panel.Children.Add(PageHeader("User Detail", "Loaded from GET /api/Users/{id}."));
        panel.Children.Add(Detail("Id", user.Id));
        panel.Children.Add(Detail("Full Name", user.FullName));
        panel.Children.Add(Detail("Email", user.Email));
        panel.Children.Add(Detail("Date of Birth", user.DateOfBirth.ToString("yyyy-MM-dd")));
        panel.Children.Add(Detail("Student Code", user.StudentCode ?? string.Empty));
        panel.Children.Add(Detail("Department", user.Department));
        panel.Children.Add(Detail("Roles", user.RolesText));
        panel.Children.Add(Detail("Locked", user.IsLocked ? "Yes" : "No"));
        AddButton(panel, "Back to Users", async () => await ShowUsersAsync(), false);
        _contentHost.Content = WpfUi.Card(panel);
    }

    private async Task LoadUsersAsync()
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Success)
        {
            ShowResult(false, result.Message);
            return;
        }

        if (_userGrid is not null)
        {
            _userGrid.ItemsSource = result.Data?.Select(u => new UserGridRow(u)).ToList() ?? new List<UserGridRow>();
        }
    }

    private async Task ChangeRoleAsync()
    {
        if (_userGrid?.SelectedItem is not UserGridRow row)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_roleComboBox?.SelectedItem is not string role)
        {
            MessageBox.Show("Please select a role.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.UpdateRoleAsync(row.Id, new UpdateUserRoleRequest { Role = role });
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private async Task LockUserAsync()
    {
        if (_userGrid?.SelectedItem is not UserGridRow row)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.LockAsync(row.Id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private async Task UnlockUserAsync()
    {
        if (_userGrid?.SelectedItem is not UserGridRow row)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.UnlockAsync(row.Id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private async Task ShowActivitiesAsync()
    {
        Activate("Activity Management");
        var root = new StackPanel();
        root.Children.Add(PageHeader("Activity Management", "Admin has full activity permissions: create, edit, and delete."));

        var toolbar = new WrapPanel { Margin = new Thickness(0, 0, 0, 12) };
        AddButton(toolbar, "Refresh", async () => await LoadActivitiesAsync(), false);
        AddButton(toolbar, "Create Activity", () => { ShowActivityForm(null); return Task.CompletedTask; }, true);
        AddButton(toolbar, "Edit Activity", () => { ShowEditSelectedActivity(); return Task.CompletedTask; }, false);
        AddButton(toolbar, "Delete Activity", async () => await DeleteActivityAsync(), true);
        root.Children.Add(toolbar);

        _activityGrid = WpfUi.Grid();
        root.Children.Add(TableCard(_activityGrid));
        _contentHost.Content = root;
        await LoadActivitiesAsync();
    }

    private void ShowEditSelectedActivity()
    {
        if (_activityGrid?.SelectedItem is not ActivityGridRow row)
        {
            MessageBox.Show("Please select an activity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ShowActivityForm(row);
    }

    private void ShowActivityForm(ActivityGridRow? existing)
    {
        Activate("Activity Management");
        var isEdit = existing is not null;
        var panel = new StackPanel { MaxWidth = 820 };
        panel.Children.Add(PageHeader(isEdit ? "Edit Activity" : "Create Activity", isEdit ? "PUT /api/ManageActivities/{id}" : "POST /api/ManageActivities"));

        var title = AddText(panel, "Title", existing?.Title);
        var description = AddText(panel, "Description", existing?.Description);
        var location = AddText(panel, "Location", existing?.Location);
        var startDate = new DatePicker { SelectedDate = existing?.StartDateTime.Date ?? DateTime.Today.AddDays(7) };
        AddControl(panel, "Start Date", startDate);
        var startTime = AddText(panel, "Start Time (HH:mm)", existing?.StartDateTime.ToString("HH:mm") ?? "09:00");
        var endDate = new DatePicker { SelectedDate = existing?.EndDateTime.Date ?? DateTime.Today.AddDays(7) };
        AddControl(panel, "End Date", endDate);
        var endTime = AddText(panel, "End Time (HH:mm)", existing?.EndDateTime.ToString("HH:mm") ?? "11:00");
        var maxParticipants = AddText(panel, "Max Participants", existing?.MaxParticipants.ToString() ?? "100");
        var type = new ComboBox
        {
            DisplayMemberPath = "Name",
            SelectedValuePath = "Id",
            ItemsSource = _activityTypes,
            SelectedValue = existing?.TypeId ?? 1
        };
        AddControl(panel, "Type", type);

        var actions = new WrapPanel { Margin = new Thickness(0, 8, 0, 0) };
        AddButton(actions, "Back to Activities", async () => await ShowActivitiesAsync(), false);
        AddButton(actions, isEdit ? "Save Changes" : "Create Activity", async () =>
        {
            var validation = ValidateActivity(title.Text, description.Text, location.Text, startDate.SelectedDate, startTime.Text, endDate.SelectedDate, endTime.Text, maxParticipants.Text, type.SelectedValue);
            if (validation is not null)
            {
                MessageBox.Show(validation, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var start = CombineDateTime(startDate.SelectedDate!.Value, startTime.Text);
            var end = CombineDateTime(endDate.SelectedDate!.Value, endTime.Text);
            var typeId = Convert.ToInt32(type.SelectedValue);
            var max = int.Parse(maxParticipants.Text.Trim());

            if (isEdit)
            {
                var result = await _activityService.UpdateAsync(existing!.Id, new UpdateActivityRequest
                {
                    Title = title.Text.Trim(),
                    Description = description.Text.Trim(),
                    Location = location.Text.Trim(),
                    StartTime = start,
                    EndTime = end,
                    MaxParticipants = max,
                    Type = typeId
                });
                ShowResult(result.Success, result.Message);
                if (result.Success) await ShowActivitiesAsync();
            }
            else
            {
                var result = await _activityService.CreateAsync(new CreateActivityRequest
                {
                    Title = title.Text.Trim(),
                    Description = description.Text.Trim(),
                    Location = location.Text.Trim(),
                    StartTime = start,
                    EndTime = end,
                    MaxParticipants = max,
                    Type = typeId
                });
                ShowResult(result.Success, result.Message);
                if (result.Success) await ShowActivitiesAsync();
            }
        }, true);
        panel.Children.Add(actions);
        _contentHost.Content = WpfUi.Card(panel);
    }

    private async Task LoadActivitiesAsync()
    {
        var result = await _activityService.GetAllAsync();
        if (!result.Success)
        {
            ShowResult(false, result.Message);
            return;
        }

        if (_activityGrid is not null)
        {
            _activityGrid.ItemsSource = result.Data?.Select(a => new ActivityGridRow(a)).ToList() ?? new List<ActivityGridRow>();
        }
    }

    private async Task DeleteActivityAsync()
    {
        if (_activityGrid?.SelectedItem is not ActivityGridRow row)
        {
            MessageBox.Show("Please select an activity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Delete activity #{row.Id}?", "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        var result = await _activityService.DeleteAsync(row.Id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadActivitiesAsync();
        }
    }

    private async Task ShowApprovalsAsync()
    {
        Activate("Activity Approval");
        var root = new StackPanel();
        root.Children.Add(PageHeader("Activity Approval", "Review pending organizer submissions. Admin uses the same approval API as Staff."));

        var toolbar = new WrapPanel { Margin = new Thickness(0, 0, 0, 12) };
        AddButton(toolbar, "Refresh", async () => await LoadPendingActivitiesAsync(), false);
        AddButton(toolbar, "Approve", async () => await ApproveSelectedActivityAsync(), true);
        AddButton(toolbar, "Reject", async () => await RejectSelectedActivityAsync(), false);
        root.Children.Add(toolbar);

        _approvalGrid = WpfUi.Grid();
        root.Children.Add(TableCard(_approvalGrid));
        _contentHost.Content = root;
        await LoadPendingActivitiesAsync();
    }

    private async Task LoadPendingActivitiesAsync()
    {
        var result = await _activityService.GetPendingAsync();
        if (!result.Success)
        {
            ShowResult(false, result.Message);
            return;
        }

        if (_approvalGrid is not null)
        {
            _approvalGrid.ItemsSource = result.Data?.Select(a => new ActivityGridRow(a)).ToList() ?? new List<ActivityGridRow>();
        }
    }

    private async Task ApproveSelectedActivityAsync()
    {
        if (_approvalGrid?.SelectedItem is not ActivityGridRow row)
        {
            MessageBox.Show("Please select a pending activity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _activityService.ApproveAsync(row.Id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadPendingActivitiesAsync();
        }
    }

    private async Task RejectSelectedActivityAsync()
    {
        if (_approvalGrid?.SelectedItem is not ActivityGridRow row)
        {
            MessageBox.Show("Please select a pending activity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _activityService.RejectAsync(row.Id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadPendingActivitiesAsync();
        }
    }

    private async Task ShowStatisticsAsync()
    {
        Activate("Statistics");
        var stats = await GetStatisticsOrDefaultAsync();

        var root = new Grid();
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var left = new StackPanel();
        left.Children.Add(PageHeader("Statistics", "A visual summary from GET /api/Statistics."));
        left.Children.Add(WpfUi.MetricCard("Total Activities", stats.TotalActivities.ToString(), WpfUi.Primary, "Activities"));
        left.Children.Add(WpfUi.MetricCard("Total Registrations", stats.TotalRegistrations.ToString(), new SolidColorBrush(Color.FromRgb(16, 185, 129)), "Registrations"));
        left.Children.Add(WpfUi.MetricCard("Participated Students", stats.ParticipatedStudents.ToString(), new SolidColorBrush(Color.FromRgb(245, 158, 11)), "Students"));
        root.Children.Add(left);

        var rightPanel = new StackPanel();
        rightPanel.Children.Add(new TextBlock
        {
            Text = "Activity Status Distribution",
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = WpfUi.Text,
            Margin = new Thickness(0, 0, 0, 14)
        });
        rightPanel.Children.Add(BuildPieChart(stats));
        rightPanel.Children.Add(Legend("Approved", stats.ApprovedActivities, new SolidColorBrush(Color.FromRgb(16, 185, 129))));
        rightPanel.Children.Add(Legend("Pending", stats.PendingActivities, new SolidColorBrush(Color.FromRgb(245, 158, 11))));
        var other = Math.Max(0, stats.TotalActivities - stats.ApprovedActivities - stats.PendingActivities);
        rightPanel.Children.Add(Legend("Other", other, new SolidColorBrush(Color.FromRgb(99, 102, 241))));
        var card = WpfUi.Card(rightPanel);
        Grid.SetColumn(card, 1);
        root.Children.Add(card);

        _contentHost.Content = root;
    }

    private Canvas BuildPieChart(AdminStatisticsResponse stats)
    {
        var canvas = new Canvas { Width = 360, Height = 300, Margin = new Thickness(0, 0, 0, 18) };
        var total = Math.Max(1, stats.TotalActivities);
        var other = Math.Max(0, stats.TotalActivities - stats.ApprovedActivities - stats.PendingActivities);
        var start = -90.0;
        start = AddSlice(canvas, start, stats.ApprovedActivities, total, new SolidColorBrush(Color.FromRgb(16, 185, 129)));
        start = AddSlice(canvas, start, stats.PendingActivities, total, new SolidColorBrush(Color.FromRgb(245, 158, 11)));
        _ = AddSlice(canvas, start, other, total, new SolidColorBrush(Color.FromRgb(99, 102, 241)));
        return canvas;
    }

    private static double AddSlice(Canvas canvas, double startAngle, int value, int total, Brush fill)
    {
        if (value <= 0)
        {
            return startAngle;
        }

        var sweep = 360.0 * value / total;
        var center = new Point(180, 145);
        const double radius = 118;
        var start = PointOnCircle(center, radius, startAngle);
        var end = PointOnCircle(center, radius, startAngle + sweep);
        var isLarge = sweep > 180;

        var figure = new PathFigure { StartPoint = center, IsClosed = true };
        figure.Segments.Add(new LineSegment(start, true));
        figure.Segments.Add(new ArcSegment(end, new Size(radius, radius), 0, isLarge, SweepDirection.Clockwise, true));
        figure.Segments.Add(new LineSegment(center, true));
        var path = new Path
        {
            Fill = fill,
            Stroke = Brushes.White,
            StrokeThickness = 2,
            Data = new PathGeometry(new[] { figure })
        };
        canvas.Children.Add(path);
        return startAngle + sweep;
    }

    private static Point PointOnCircle(Point center, double radius, double angleDegrees)
    {
        var radians = Math.PI * angleDegrees / 180.0;
        return new Point(center.X + radius * Math.Cos(radians), center.Y + radius * Math.Sin(radians));
    }

    private async Task<AdminStatisticsResponse> GetStatisticsOrDefaultAsync()
    {
        var result = await _statisticsService.GetStatisticsAsync();
        return result.Success && result.Data is not null ? result.Data : new AdminStatisticsResponse();
    }

    private StackPanel PageHeader(string title, string subtitle)
    {
        var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 18) };
        panel.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 26,
            FontWeight = FontWeights.Bold,
            Foreground = WpfUi.Text
        });
        panel.Children.Add(new TextBlock
        {
            Text = subtitle,
            Foreground = WpfUi.Muted,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 4, 0, 0)
        });
        return panel;
    }

    private static void AddButton(Panel panel, string text, Func<Task> action, bool primary)
    {
        var button = primary ? WpfUi.PrimaryButton(text) : WpfUi.SecondaryButton(text);
        button.Click += async (_, _) => await action();
        panel.Children.Add(button);
    }

    private static TextBox AddText(Panel panel, string label, string? value = null)
    {
        var box = new TextBox { Text = value ?? string.Empty };
        AddControl(panel, label, box);
        return box;
    }

    private static PasswordBox AddPassword(Panel panel, string label)
    {
        var box = new PasswordBox();
        AddControl(panel, label, box);
        return box;
    }

    private static void AddControl(Panel panel, string label, Control control)
    {
        panel.Children.Add(new TextBlock { Text = label, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(control);
    }

    private static string? ValidateCreate(string fullName, string email, string password, DateTime? dob, string department, object? role)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "Full Name is required.";
        if (string.IsNullOrWhiteSpace(email)) return "Email is required.";
        if (!IsValidEmail(email.Trim())) return "Email format is invalid.";
        if (string.IsNullOrWhiteSpace(password)) return "Password is required.";
        if (password.Length < 6) return "Password must be at least 6 characters.";
        if (dob is null) return "Date of Birth is required.";
        if (string.IsNullOrWhiteSpace(department)) return "Department is required.";
        if (role is null) return "Role is required.";
        return null;
    }

    private static string? ValidateActivity(
        string title,
        string description,
        string location,
        DateTime? startDate,
        string startTime,
        DateTime? endDate,
        string endTime,
        string maxParticipants,
        object? type)
    {
        if (string.IsNullOrWhiteSpace(title)) return "Title is required.";
        if (string.IsNullOrWhiteSpace(description)) return "Description is required.";
        if (string.IsNullOrWhiteSpace(location)) return "Location is required.";
        if (startDate is null) return "Start date is required.";
        if (endDate is null) return "End date is required.";
        if (!TimeSpan.TryParse(startTime, out _)) return "Start time must use HH:mm format.";
        if (!TimeSpan.TryParse(endTime, out _)) return "End time must use HH:mm format.";
        if (!int.TryParse(maxParticipants, out var max) || max is < 1 or > 1000) return "Max Participants must be from 1 to 1000.";
        if (type is null) return "Type is required.";

        var start = CombineDateTime(startDate.Value, startTime);
        var end = CombineDateTime(endDate.Value, endTime);
        if (end <= start) return "End time must be greater than start time.";
        return null;
    }

    private static DateTime CombineDateTime(DateTime date, string timeText)
    {
        return date.Date.Add(TimeSpan.Parse(timeText.Trim()));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Border Note(string text)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 0, 0, 12),
            Child = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap }
        };
    }

    private static Border StatLine(string title, string value)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = title, Foreground = WpfUi.Muted, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(new TextBlock { Text = value, FontSize = 18, FontWeight = FontWeights.Bold, Foreground = WpfUi.Text });
        return new Border
        {
            Child = panel,
            BorderBrush = WpfUi.PrimaryLight,
            BorderThickness = new Thickness(4, 0, 0, 0),
            Background = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 16)
        };
    }

    private static Border Detail(string label, string value)
    {
        return Note($"{label}: {value}");
    }

    private static Border TableCard(UIElement content)
    {
        return new Border
        {
            Child = content,
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(0),
            MinHeight = 480
        };
    }

    private static Border Legend(string label, int value, Brush color)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 0) };
        row.Children.Add(new Border { Width = 16, Height = 16, Background = color, CornerRadius = new CornerRadius(4), Margin = new Thickness(0, 2, 10, 0) });
        row.Children.Add(new TextBlock { Text = $"{label}: {value}", FontWeight = FontWeights.SemiBold, Foreground = WpfUi.Text });
        return new Border { Child = row };
    }

    private Border PanelCard(string title, UIElement content)
    {
        var panel = new DockPanel();
        var header = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(18, 12, 18, 12),
            Child = new TextBlock { Text = title, FontSize = 19, FontWeight = FontWeights.Bold, Foreground = WpfUi.Text }
        };
        DockPanel.SetDock(header, Dock.Top);
        panel.Children.Add(header);
        panel.Children.Add(new Border { Padding = new Thickness(20), Child = content });

        return new Border
        {
            Child = panel,
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 18, 0),
            MinHeight = 420
        };
    }

    private static void ShowResult(bool success, string message)
    {
        MessageBox.Show(message, success ? "Success" : "Error", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);
    }

    private void Logout()
    {
        TokenStorage.Clear();
        new LoginWindow().Show();
        Close();
    }

    private sealed class UserGridRow
    {
        public UserGridRow(UserResponse user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            DateOfBirth = user.DateOfBirth.ToString("yyyy-MM-dd");
            StudentCode = user.StudentCode ?? string.Empty;
            Department = user.Department;
            Roles = user.RolesText;
            IsLocked = user.IsLocked;
        }

        public string Id { get; }
        public string FullName { get; }
        public string Email { get; }
        public string DateOfBirth { get; }
        public string StudentCode { get; }
        public string Department { get; }
        public string Roles { get; }
        public bool IsLocked { get; }
    }

    private sealed class ActivityGridRow
    {
        public ActivityGridRow(ActivityResponse activity)
        {
            Id = activity.Id;
            Title = activity.Title;
            Description = activity.Description;
            Location = activity.Location;
            StartDateTime = activity.StartTime;
            EndDateTime = activity.EndTime;
            StartTime = activity.StartTime.ToString("g");
            EndTime = activity.EndTime.ToString("g");
            MaxParticipants = activity.MaxParticipants;
            TypeId = activity.Type;
            Type = activity.TypeText;
            Status = activity.StatusText;
            OrganizerName = activity.OrganizerName;
        }

        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string Location { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public string StartTime { get; }
        public string EndTime { get; }
        public int MaxParticipants { get; }
        public int TypeId { get; }
        public string Type { get; }
        public string Status { get; }
        public string OrganizerName { get; }
    }

    private sealed record ActivityTypeOption(int Id, string Name);
}
