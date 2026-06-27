using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Users;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class CreateUserWindow : Window
{
    private readonly UserClientService _userService = new();
    private readonly TextBox _fullNameTextBox = new();
    private readonly TextBox _emailTextBox = new();
    private readonly PasswordBox _passwordBox = new();
    private readonly DatePicker _dateOfBirthPicker = new();
    private readonly TextBox _studentCodeTextBox = new();
    private readonly TextBox _departmentTextBox = new();
    private readonly ComboBox _roleComboBox = new();

    public CreateUserWindow()
    {
        WpfUi.ApplyWindow(this, "Create User", 560, 660);

        var root = new ScrollViewer { Content = BuildForm(), Margin = new Thickness(28) };
        Content = root;
    }

    private StackPanel BuildForm()
    {
        var panel = new StackPanel();
        panel.Children.Add(WpfUi.Heading("Create User"));
        AddField(panel, "FullName", _fullNameTextBox);
        AddField(panel, "Email", _emailTextBox);
        AddField(panel, "Password", _passwordBox);
        _dateOfBirthPicker.SelectedDate = DateTime.Today.AddYears(-20);
        AddField(panel, "DateOfBirth", _dateOfBirthPicker);
        AddField(panel, "StudentCode", _studentCodeTextBox);
        AddField(panel, "Department", _departmentTextBox);
        _roleComboBox.ItemsSource = new[] { "Admin", "Staff", "Organizer", "Student" };
        AddField(panel, "Role", _roleComboBox);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var cancelButton = WpfUi.SecondaryButton("Cancel");
        cancelButton.Click += (_, _) => Close();
        var createButton = WpfUi.PrimaryButton("Create");
        createButton.Click += async (_, _) => await CreateAsync();
        buttons.Children.Add(cancelButton);
        buttons.Children.Add(createButton);
        panel.Children.Add(buttons);
        return panel;
    }

    private static void AddField(Panel panel, string label, Control control)
    {
        panel.Children.Add(new TextBlock { Text = label, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(control);
    }

    private async Task CreateAsync()
    {
        var validation = ValidateForm();
        if (validation is not null)
        {
            MessageBox.Show(validation, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var request = new CreateUserRequest
        {
            FullName = _fullNameTextBox.Text.Trim(),
            Email = _emailTextBox.Text.Trim(),
            Password = _passwordBox.Password,
            DateOfBirth = _dateOfBirthPicker.SelectedDate!.Value.Date,
            StudentCode = string.IsNullOrWhiteSpace(_studentCodeTextBox.Text) ? null : _studentCodeTextBox.Text.Trim(),
            Department = _departmentTextBox.Text.Trim(),
            Role = _roleComboBox.SelectedItem?.ToString() ?? string.Empty
        };

        var result = await _userService.CreateUserAsync(request);
        MessageBox.Show(result.Message, result.Success ? "Success" : "Error", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Error);
        if (result.Success)
        {
            DialogResult = true;
            Close();
        }
    }

    private string? ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(_fullNameTextBox.Text)) return "FullName is required.";
        if (string.IsNullOrWhiteSpace(_emailTextBox.Text)) return "Email is required.";
        if (!IsValidEmail(_emailTextBox.Text.Trim())) return "Email format is invalid.";
        if (string.IsNullOrWhiteSpace(_passwordBox.Password)) return "Password is required.";
        if (_passwordBox.Password.Length < 6) return "Password must be at least 6 characters.";
        if (_dateOfBirthPicker.SelectedDate is null) return "DateOfBirth is required.";
        if (string.IsNullOrWhiteSpace(_departmentTextBox.Text)) return "Department is required.";
        if (_roleComboBox.SelectedItem is null) return "Role is required.";
        return null;
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
}
