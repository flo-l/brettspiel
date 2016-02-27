using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Client;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      private void Application_Startup(object sender, StartupEventArgs e)
      {
        var mainWindow = new MainWindow();

        var vm = new MainWindowViewModel();
        mainWindow.DataContext = vm;
        mainWindow.Show();
      }
    }
}
