using System.Windows;
using System.Windows.Controls;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Users;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class UserDetailWindow : Window
{
    private readonly string _userId;
    private readonly UserClientService _userService = new();
    private readonly StackPanel _detailPanel = new();

    public UserDetailWindow(string userId)
    {
        _userId = userId;
        WpfUi.ApplyWindow(this, "User Detail", 650, 520);

        var root = new StackPanel { Margin = new Thickness(28) };
        root.Children.Add(WpfUi.Heading("User Detail"));
        root.Children.Add(WpfUi.Card(_detailPanel));
        var closeButton = WpfUi.SecondaryButton("Close");
        closeButton.Click += (_, _) => Close();
        root.Children.Add(closeButton);
        Content = root;

        Loaded += async (_, _) => await LoadDetailAsync();
    }

    private async Task LoadDetailAsync()
    {
        var result = await _userService.GetUserAsync(_userId);
        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        Render(result.Data);
    }

    private void Render(UserResponse user)
    {
        _detailPanel.Children.Clear();
        AddRow("Id", user.Id);
        AddRow("FullName", user.FullName);
        AddRow("Email", user.Email);
        AddRow("DateOfBirth", user.DateOfBirth.ToString("yyyy-MM-dd"));
        AddRow("StudentCode", user.StudentCode ?? string.Empty);
        AddRow("Department", user.Department);
        AddRow("Roles", user.RolesText);
        AddRow("IsLocked", user.IsLocked ? "Yes" : "No");
    }

    private void AddRow(string label, string value)
    {
        _detailPanel.Children.Add(new TextBlock
        {
            Text = $"{label}: {value}",
            Margin = new Thickness(0, 5, 0, 5),
            TextWrapping = TextWrapping.Wrap
        });
    }
}
