using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KTrade.Server.UI.Windows;

namespace KTrade.Server
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
