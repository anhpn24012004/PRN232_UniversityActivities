using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopApp_Client.Wpf.Helpers;

public static class WpfUi
{
    public static readonly Brush PageBackground = new SolidColorBrush(Color.FromRgb(250, 247, 255));
    public static readonly Brush Primary = new SolidColorBrush(Color.FromRgb(139, 92, 246));
    public static readonly Brush PrimaryLight = new SolidColorBrush(Color.FromRgb(196, 181, 253));
    public static readonly Brush PrimaryDark = new SolidColorBrush(Color.FromRgb(109, 40, 217));
    public static readonly Brush Danger = new SolidColorBrush(Color.FromRgb(220, 38, 38));
    public static readonly Brush Text = new SolidColorBrush(Color.FromRgb(30, 41, 59));
    public static readonly Brush Muted = new SolidColorBrush(Color.FromRgb(100, 116, 139));

    public static void ApplyWindow(Window window, string title, double width, double height)
    {
        window.Title = title;
        window.Width = width;
        window.Height = height;
        window.MinWidth = Math.Min(width, 900);
        window.MinHeight = Math.Min(height, 520);
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.Background = PageBackground;
        window.FontFamily = new FontFamily("Segoe UI");
        window.FontSize = 14;
    }

    public static Grid AppShell(
        Window window,
        string active,
        UIElement content,
        Action overview,
        Action users,
        Action activities,
        Action statistics,
        Action logout)
    {
        var root = new Grid { Background = PageBackground };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(260) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var sidebar = new Border
        {
            Background = new LinearGradientBrush(
                Color.FromRgb(139, 92, 246),
                Color.FromRgb(216, 180, 254),
                90),
            Padding = new Thickness(20, 26, 20, 20)
        };

        var nav = new StackPanel();
        var brand = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(6, 0, 0, 28)
        };
        brand.Children.Add(new Image
        {
            Source = new BitmapImage(new Uri("Assets/ua-logo.png", UriKind.Relative)),
            Width = 52,
            Height = 52,
            Margin = new Thickness(0, 0, 12, 0)
        });
        brand.Children.Add(new TextBlock
        {
            Text = "UA Admin",
            Foreground = Brushes.White,
            FontSize = 25,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center
        });
        nav.Children.Add(brand);
        nav.Children.Add(SidebarButton("Overview", active == "Overview", overview));
        nav.Children.Add(SidebarButton("User Management", active == "Users", users));
        nav.Children.Add(SidebarButton("Activity Management", active == "Activities", activities));
        nav.Children.Add(SidebarButton("Statistics", active == "Statistics", statistics));
        sidebar.Child = nav;
        root.Children.Add(sidebar);

        var main = new Grid { Margin = new Thickness(34, 28, 34, 28) };
        main.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        main.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var topbar = new Border
        {
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20, 14, 20, 14),
            Margin = new Thickness(0, 0, 0, 20),
            Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Color.FromRgb(15, 23, 42),
                BlurRadius = 18,
                ShadowDepth = 2,
                Opacity = 0.08
            }
        };

        var topbarGrid = new Grid();
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        topbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        topbarGrid.Children.Add(new TextBlock
        {
            Text = window.Title,
            Foreground = Text,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        });
        var user = new TextBlock
        {
            Text = "Admin",
            Foreground = Text,
            Margin = new Thickness(0, 0, 16, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        System.Windows.Controls.Grid.SetColumn(user, 1);
        topbarGrid.Children.Add(user);
        var logoutButton = DangerButton("Logout");
        logoutButton.Click += (_, _) => logout();
        System.Windows.Controls.Grid.SetColumn(logoutButton, 2);
        topbarGrid.Children.Add(logoutButton);
        topbar.Child = topbarGrid;
        main.Children.Add(topbar);

        var contentHost = new ScrollViewer
        {
            Content = content,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        System.Windows.Controls.Grid.SetRow(contentHost, 1);
        main.Children.Add(contentHost);

        System.Windows.Controls.Grid.SetColumn(main, 1);
        root.Children.Add(main);
        return root;
    }

    public static TextBlock Heading(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = Text,
            Margin = new Thickness(0, 0, 0, 8)
        };
    }

    public static TextBlock MutedText(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = Muted,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 18)
        };
    }

    public static Button PrimaryButton(string text)
    {
        return new Button
        {
            Content = text,
            Background = Primary,
            Foreground = Brushes.White,
            BorderBrush = PrimaryDark,
            Padding = new Thickness(16, 8, 16, 8)
        };
    }

    public static Button SecondaryButton(string text)
    {
        return new Button
        {
            Content = text,
            Background = Brushes.White,
            Foreground = PrimaryDark,
            BorderBrush = PrimaryLight,
            Padding = new Thickness(16, 8, 16, 8)
        };
    }

    public static Button DangerButton(string text)
    {
        return new Button
        {
            Content = text,
            Background = new SolidColorBrush(Color.FromRgb(244, 63, 94)),
            Foreground = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(244, 63, 94)),
            MinWidth = 112
        };
    }

    public static Button SidebarButton(string text, bool active, Action action)
    {
        var button = new Button
        {
            Content = text,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Background = active ? new SolidColorBrush(Color.FromArgb(78, 255, 255, 255)) : Brushes.Transparent,
            Foreground = Brushes.White,
            BorderBrush = Brushes.Transparent,
            FontSize = 16,
            FontWeight = FontWeights.SemiBold,
            Padding = new Thickness(18, 12, 18, 12),
            Margin = new Thickness(0, 4, 0, 8)
        };
        button.Click += (_, _) => action();
        return button;
    }

    public static DataGrid Grid()
    {
        return new DataGrid
        {
            AutoGenerateColumns = true,
            CanUserAddRows = false,
            IsReadOnly = true,
            SelectionMode = DataGridSelectionMode.Single,
            SelectionUnit = DataGridSelectionUnit.FullRow,
            Background = Brushes.White,
            RowBackground = Brushes.White,
            AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(250, 247, 255)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(221, 214, 254)),
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 10, 0, 0)
        };
    }

    public static Border Card(UIElement child)
    {
        return new Border
        {
            Child = child,
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(221, 214, 254)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(24),
            Margin = new Thickness(0, 0, 0, 14)
        };
    }

    public static Border MetricCard(string title, string value, Brush accent, string footer)
    {
        var panel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
        panel.Children.Add(new TextBlock
        {
            Text = title,
            Foreground = Muted,
            FontWeight = FontWeights.SemiBold,
            TextAlignment = TextAlignment.Center
        });
        panel.Children.Add(new TextBlock
        {
            Text = value,
            Foreground = Text,
            FontSize = 34,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 8, 0, 4)
        });
        panel.Children.Add(new TextBlock
        {
            Text = footer,
            Foreground = accent,
            TextAlignment = TextAlignment.Center
        });

        return new Border
        {
            Child = panel,
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(22),
            Margin = new Thickness(0, 0, 18, 18),
            BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
            BorderThickness = new Thickness(1)
        };
    }
}
