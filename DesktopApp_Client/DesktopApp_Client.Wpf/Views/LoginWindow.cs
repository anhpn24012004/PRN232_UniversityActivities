using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Auth;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class LoginWindow : Window
{
    private readonly AuthClientService _authService = new();
    private readonly TextBox _emailTextBox = new();
    private readonly PasswordBox _passwordBox = new();
    private readonly Button _loginButton;

    public LoginWindow()
    {
        WpfUi.ApplyWindow(this, "Admin Login - DesktopApp Client", 520, 460);

        var root = new Grid { Margin = new Thickness(54) };
        var cardPanel = new StackPanel();
        cardPanel.Children.Add(new Image
        {
            Source = new BitmapImage(new Uri("Assets/ua-logo.png", UriKind.Relative)),
            Width = 86,
            Height = 86,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 12)
        });
        cardPanel.Children.Add(WpfUi.Heading("Admin Login"));
        cardPanel.Children.Add(WpfUi.MutedText("University Activities desktop client for Admin."));
        cardPanel.Children.Add(new TextBlock { Text = "Email", FontWeight = FontWeights.SemiBold });
        cardPanel.Children.Add(_emailTextBox);
        cardPanel.Children.Add(new TextBlock { Text = "Password", FontWeight = FontWeights.SemiBold });
        cardPanel.Children.Add(_passwordBox);

        _loginButton = WpfUi.PrimaryButton("Login");
        _loginButton.Click += async (_, _) => await LoginAsync();
        cardPanel.Children.Add(_loginButton);

        root.Children.Add(WpfUi.Card(cardPanel));
        Content = root;
    }

    private async Task LoginAsync()
    {
        var email = _emailTextBox.Text.Trim();
        var password = _passwordBox.Password;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Email and password are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _loginButton.IsEnabled = false;
        try
        {
            var result = await _authService.LoginAsync(new LoginRequest { Email = email, Password = password });
            if (!result.Success || result.Data is null)
            {
                MessageBox.Show(result.Message, "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!result.Data.Roles.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This account does not have Admin permission.", "Access denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TokenStorage.Set(result.Data.Token, result.Data.Email, result.Data.FullName, result.Data.Roles);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        finally
        {
            _loginButton.IsEnabled = true;
        }
    }
}
