using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using KTrade.Core;

namespace KTrade.Server.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameConfig _gameConfig = new GameConfig
        {
            AuctionCooldown = 40,
            BaseResourcesAmount = 4,
            StorageAmountForLevel = 200,
            StorageCostForLevel = 20,
            StartingMoney = 100,
            ResourceOrderFromTickMultiplier = 1.5,
            ResourceAuctionFromTickMultiplier = 1,
            MoneyForWin = 5000,
            BonusInterval = 60,
            BonusAmount = 50,
            OrdersAmountFromPlayerAmountMultiplier = 1.5
        };

        private GameSession _gameSession;

        private readonly DispatcherTimer _timer;

        private readonly IList<Player> _players = new List<Player>();

        private ServiceHost _serviceHost;

        public MainWindow()
        {
            InitializeComponent();

            ConfigControl.DataContext = _gameConfig;
            StartButton.Click += StartButton_Click;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += _timer_Tick;

            Closed += MainWindow_Closed;

            PlayerController.PlayerConnected += PlayerController_PlayerConnected;

            var baseAddress = new Uri($"net.tcp://{GetLocalIpAddress()}:{1488}/ktrade");
            _serviceHost = new ServiceHost(typeof(PlayerController));
            _serviceHost.AddServiceEndpoint(typeof(IPlayerController), new NetTcpBinding(SecurityMode.None), baseAddress);
            _serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());

            _serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            _serviceHost.Open();
        }

        private string GetLocalIpAddress()
        {
            return "192.168.1.249";
            /*var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");*/
        }

        private void PlayerController_PlayerConnected(Player player)
        {
            _players.Add(player);
            PlayersList.Dispatcher.Invoke(() =>
            {
                PlayersList.Items.Add(player.Name);
            });
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _timer.Stop();
            _serviceHost.Close();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _gameSession.Tick();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigControl.IsEnabled = false;
            StartButton.IsEnabled = false;
            for (int i = 0; i < PlayersList.Items.Count; ++i)
                _players[i].Name = (string)PlayersList.Items[i];
            _gameSession = new GameSession(_players, _gameConfig);
            _timer.Start();
        }
    }
}
