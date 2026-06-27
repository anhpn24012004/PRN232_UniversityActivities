using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DesktopApp_Client.Wpf.Helpers;
using DesktopApp_Client.Wpf.Models.Statistics;
using DesktopApp_Client.Wpf.Services;

namespace DesktopApp_Client.Wpf.Views;

public class StatisticsWindow : Window
{
    private readonly StatisticsClientService _statisticsService = new();
    private readonly UniformGrid _cardsGrid = new() { Columns = 2 };

    public StatisticsWindow()
    {
        WpfUi.ApplyWindow(this, "Statistics", 760, 540);

        var root = new StackPanel { Margin = new Thickness(28) };
        root.Children.Add(WpfUi.Heading("Admin Statistics"));
        root.Children.Add(WpfUi.MutedText("Live statistics loaded from /api/Statistics."));
        root.Children.Add(_cardsGrid);

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
        var backButton = WpfUi.SecondaryButton("Back to Home");
        backButton.Click += (_, _) => Close();
        var refreshButton = WpfUi.PrimaryButton("Refresh");
        refreshButton.Click += async (_, _) => await LoadStatisticsAsync();
        toolbar.Children.Add(backButton);
        toolbar.Children.Add(refreshButton);
        root.Children.Add(toolbar);
        Content = WpfUi.AppShell(
            this,
            "Statistics",
            root,
            overview: Close,
            users: () =>
            {
                new UserManagementWindow { Owner = Owner }.Show();
                Close();
            },
            activities: () =>
            {
                new ActivityManagementWindow { Owner = Owner }.Show();
                Close();
            },
            statistics: () => { },
            logout: Logout);

        Loaded += async (_, _) => await LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        var result = await _statisticsService.GetStatisticsAsync();
        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Render(result.Data);
    }

    private void Render(AdminStatisticsResponse statistics)
    {
        _cardsGrid.Children.Clear();
        AddCard("Total Activities", statistics.TotalActivities);
        AddCard("Approved Activities", statistics.ApprovedActivities);
        AddCard("Pending Activities", statistics.PendingActivities);
        AddCard("Total Registrations", statistics.TotalRegistrations);
        AddCard("Participated Students", statistics.ParticipatedStudents);
    }

    private void AddCard(string label, int value)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock
        {
            Text = value.ToString(),
            FontSize = 34,
            FontWeight = FontWeights.Bold,
            Foreground = WpfUi.PrimaryDark
        });
        panel.Children.Add(new TextBlock
        {
            Text = label,
            FontWeight = FontWeights.SemiBold,
            Foreground = WpfUi.Text,
            Margin = new Thickness(0, 4, 0, 0)
        });

        _cardsGrid.Children.Add(new Border
        {
            Child = panel,
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(221, 214, 254)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20),
            Margin = new Thickness(8)
        });
    }

    private void Logout()
    {
        TokenStorage.Clear();
        Owner?.Close();
        new LoginWindow().Show();
        Close();
    }
}
