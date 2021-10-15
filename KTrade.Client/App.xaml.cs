using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using KTrade.Client.UI.Windows;
using KTrade.Client.VM;
using KTrade.Core;

namespace KTrade.Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            var lobbyWindow = new LobbyWindow();
            lobbyWindow.Show();
        }
    }
}
