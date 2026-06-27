using System.Windows;
using System.Windows.Controls;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Users;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class UserManagementWindow : Window
{
    private readonly UserClientService _userService = new();
    private readonly DataGrid _grid = WpfUi.Grid();
    private readonly ComboBox _roleComboBox = new();

    public UserManagementWindow()
    {
        WpfUi.ApplyWindow(this, "User Management", 1180, 680);

        var root = new DockPanel { Margin = new Thickness(22) };

        var top = new StackPanel();
        top.Children.Add(WpfUi.Heading("User Management"));

        var toolbar = new WrapPanel { Margin = new Thickness(0, 8, 0, 12) };
        AddToolbarButton(toolbar, "Back to Home", () => { Close(); return Task.CompletedTask; }, false);
        AddToolbarButton(toolbar, "Refresh", async () => await LoadUsersAsync(), false);
        AddToolbarButton(toolbar, "Create User", async () =>
        {
            var window = new CreateUserWindow { Owner = this };
            if (window.ShowDialog() == true)
            {
                await LoadUsersAsync();
            }
        }, true);
        AddToolbarButton(toolbar, "View Detail", () => { ViewDetail(); return Task.CompletedTask; }, false);

        _roleComboBox.Width = 150;
        _roleComboBox.ItemsSource = new[] { "Admin", "Staff", "Organizer", "Student" };
        toolbar.Children.Add(new TextBlock { Text = "Role:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 6, 0) });
        toolbar.Children.Add(_roleComboBox);
        AddToolbarButton(toolbar, "Change Role", async () => await ChangeRoleAsync(), false);
        AddToolbarButton(toolbar, "Lock", async () => await LockAsync(), false);
        AddToolbarButton(toolbar, "Unlock", async () => await UnlockAsync(), false);

        top.Children.Add(toolbar);
        DockPanel.SetDock(top, Dock.Top);
        root.Children.Add(top);
        root.Children.Add(_grid);

        Content = WpfUi.AppShell(
            this,
            "Users",
            root,
            overview: Close,
            users: () => { },
            activities: () =>
            {
                new ActivityManagementWindow { Owner = Owner }.Show();
                Close();
            },
            statistics: () =>
            {
                new StatisticsWindow { Owner = Owner }.Show();
                Close();
            },
            logout: Logout);
        Loaded += async (_, _) => await LoadUsersAsync();
    }

    private static void AddToolbarButton(WrapPanel toolbar, string text, Func<Task> action, bool primary)
    {
        var button = primary ? WpfUi.PrimaryButton(text) : WpfUi.SecondaryButton(text);
        button.Click += async (_, _) => await action();
        toolbar.Children.Add(button);
    }

    private async Task LoadUsersAsync()
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Success)
        {
            ShowError(result.Message);
            return;
        }

        _grid.ItemsSource = result.Data?.Select(u => new UserGridRow(u)).ToList() ?? new List<UserGridRow>();
    }

    private void ViewDetail()
    {
        var id = SelectedUserId();
        if (id is null)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        new UserDetailWindow(id) { Owner = this }.ShowDialog();
    }

    private async Task ChangeRoleAsync()
    {
        var id = SelectedUserId();
        if (id is null)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_roleComboBox.SelectedItem is not string role)
        {
            MessageBox.Show("Please select a role.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.UpdateRoleAsync(id, new UpdateUserRoleRequest { Role = role });
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private async Task LockAsync()
    {
        var id = SelectedUserId();
        if (id is null)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.LockAsync(id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private async Task UnlockAsync()
    {
        var id = SelectedUserId();
        if (id is null)
        {
            MessageBox.Show("Please select a user first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _userService.UnlockAsync(id);
        ShowResult(result.Success, result.Message);
        if (result.Success)
        {
            await LoadUsersAsync();
        }
    }

    private string? SelectedUserId()
    {
        return _grid.SelectedItem is UserGridRow row ? row.Id : null;
    }

    private static void ShowResult(bool success, string message)
    {
        MessageBox.Show(message, success ? "Success" : "Error", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);
    }

    private static void ShowError(string message)
    {
        MessageBox.Show(string.IsNullOrWhiteSpace(message) ? "API request failed." : message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void Logout()
    {
        TokenStorage.Clear();
        Owner?.Close();
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
}
