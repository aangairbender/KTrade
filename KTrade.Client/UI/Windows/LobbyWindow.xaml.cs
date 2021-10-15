using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KTrade.Client.Common;
using KTrade.Client.VM;
using KTrade.Core;

namespace KTrade.Client.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private Player _me;
        private MainWindow _mainWindow;
        private MainVM _mainVM;

        private ICommunicationObject _factory;

        public LobbyWindow()
        {
            InitializeComponent();
            Loaded += LobbyWindow_Loaded;
        }

        private async void LobbyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var uri = await SearchServer();
            _me = CreatePlayer(uri);
            _me.PlayerController.Hello("");
            JoinButton.Content = "Сервер найден. Ждем начала игры";
            _me.PlayerControllerCallback.GameSessionUpdatedCalled += PlayerControllerCallback_GameSessionUpdatedCalled;
        }

        private void PlayerControllerCallback_GameSessionUpdatedCalled(PlayerGameSession obj)
        {
            _me.PlayerControllerCallback.GameSessionUpdatedCalled -= PlayerControllerCallback_GameSessionUpdatedCalled;

            _me.Name = obj.Player.Name;
            obj.Player = _me;

            _mainVM = new MainVM(obj);
            Dispatcher.Invoke(() =>
            {
                _mainWindow = new MainWindow { DataContext = _mainVM };
                _mainWindow.Show();
                this.Hide();
            });
        }

        private Player CreatePlayer(Uri uri)
        {
            var callback = new PlayerControllerCallback();
            var context = new InstanceContext(callback);
            var binding = new NetTcpBinding(SecurityMode.None);
            var factory = new DuplexChannelFactory<IPlayerController>(context, binding);

            var playerController = factory.CreateChannel(new EndpointAddress(uri.ToString()));

            _factory = factory;

            return new Player
            {
                PlayerController = playerController,
                PlayerControllerCallback = callback,
                Name = "Меченый"
            };
        }

        private async Task<Uri> SearchServer()
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            var findCriteria = new FindCriteria(typeof(IPlayerController));
            findCriteria.Duration = TimeSpan.FromSeconds(1);
            while (true)
            {
                var findResponse = await discoveryClient.FindTaskAsync(findCriteria);
                if (findResponse.Endpoints.Count == 0)
                    continue;

                return findResponse.Endpoints[0].Address.Uri;
            }
        }
    }
}
