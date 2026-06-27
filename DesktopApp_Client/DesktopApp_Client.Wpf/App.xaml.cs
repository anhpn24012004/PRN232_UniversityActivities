using System.Windows;
using DesktopApp_Client.Wpf.Views;

namespace DesktopApp_Client.Wpf;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        new LoginWindow().Show();
    }
}
