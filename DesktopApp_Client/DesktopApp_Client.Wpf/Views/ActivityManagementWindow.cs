using System.Windows;
using System.Windows.Controls;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Activities;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class ActivityManagementWindow : Window
{
    private readonly ActivityClientService _activityService = new();
    private readonly DataGrid _grid = WpfUi.Grid();

    public ActivityManagementWindow()
    {
        WpfUi.ApplyWindow(this, "Activity Management", 1220, 680);

        var root = new DockPanel { Margin = new Thickness(22) };
        var top = new StackPanel();
        top.Children.Add(WpfUi.Heading("Activity Management"));

        var toolbar = new WrapPanel { Margin = new Thickness(0, 8, 0, 12) };
        var backButton = WpfUi.SecondaryButton("Back to Home");
        backButton.Click += (_, _) => Close();
        var refreshButton = WpfUi.SecondaryButton("Refresh");
        refreshButton.Click += async (_, _) => await LoadActivitiesAsync();
        var deleteButton = WpfUi.PrimaryButton("Delete Activity");
        deleteButton.Click += async (_, _) => await DeleteAsync();
        toolbar.Children.Add(backButton);
        toolbar.Children.Add(refreshButton);
        toolbar.Children.Add(deleteButton);

        top.Children.Add(toolbar);
        DockPanel.SetDock(top, Dock.Top);
        root.Children.Add(top);
        root.Children.Add(_grid);
        Content = WpfUi.AppShell(
            this,
            "Activities",
            root,
            overview: Close,
            users: () =>
            {
                new UserManagementWindow { Owner = Owner }.Show();
                Close();
            },
            activities: () => { },
            statistics: () =>
            {
                new StatisticsWindow { Owner = Owner }.Show();
                Close();
            },
            logout: Logout);

        Loaded += async (_, _) => await LoadActivitiesAsync();
    }

    private async Task LoadActivitiesAsync()
    {
        var result = await _activityService.GetAllAsync();
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _grid.ItemsSource = result.Data?.Select(a => new ActivityGridRow(a)).ToList() ?? new List<ActivityGridRow>();
    }

    private async Task DeleteAsync()
    {
        var id = SelectedActivityId();
        if (id is null)
        {
            MessageBox.Show("Please select an activity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Delete activity #{id}? This action cannot be undone.",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        var result = await _activityService.DeleteAsync(id.Value);
        MessageBox.Show(result.Message, result.Success ? "Success" : "Error", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Error);
        if (result.Success)
        {
            await LoadActivitiesAsync();
        }
    }

    private int? SelectedActivityId()
    {
        return _grid.SelectedItem is ActivityGridRow row ? row.Id : null;
    }

    private void Logout()
    {
        TokenStorage.Clear();
        Owner?.Close();
        new LoginWindow().Show();
        Close();
    }

    private sealed class ActivityGridRow
    {
        public ActivityGridRow(ActivityResponse activity)
        {
            Id = activity.Id;
            Title = activity.Title;
            Description = activity.Description;
            Location = activity.Location;
            StartTime = activity.StartTime.ToString("g");
            EndTime = activity.EndTime.ToString("g");
            MaxParticipants = activity.MaxParticipants;
            Type = activity.TypeText;
            Status = activity.StatusText;
            OrganizerName = activity.OrganizerName;
        }

        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string Location { get; }
        public string StartTime { get; }
        public string EndTime { get; }
        public int MaxParticipants { get; }
        public string Type { get; }
        public string Status { get; }
        public string OrganizerName { get; }
    }
}
