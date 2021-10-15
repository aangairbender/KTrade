using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using KTrade.Client.Annotations;
using KTrade.Client.Common;
using KTrade.Core;

namespace KTrade.Client.VM
{
    public class MainVM : INotifyPropertyChanged
    {
        private Player _me;

        private PlayerGameSession _playerGameSession;

        private List<OrderVM> _orders;
        private List<TradeAsItemVm> _trades;

        public Player Player => _playerGameSession.Player;
        public PlayerData PlayerData => _playerGameSession.PlayerData;
        public List<OrderVM> Orders => _orders;
        public List<TradeAsItemVm> Trades => _trades;
        public GameConfig GameConfig => _playerGameSession.GameConfig;
        public int CurrentTick => _playerGameSession.CurrentTick;
        public Dictionary<Resource, int> AuctionAmounts => _playerGameSession.AuctionAmounts;

        public AuctionVM AuctionVM { get; }

        public int TimeToAuction =>
            (CurrentTick + GameConfig.AuctionCooldown - 1) / GameConfig.AuctionCooldown * GameConfig.AuctionCooldown -
            CurrentTick;

        public TradeVM TradeVM { get; }

        public ObservableCollection<KeyValuePair<int, string>> Logs { get; } = new ObservableCollection<KeyValuePair<int, string>>();

        public RelayCommand UpgradeStorageCommand { get; }

        public MainVM(PlayerGameSession playerGameSession)
        {
            Resource.SetBaseAmount(playerGameSession.GameConfig.BaseResourcesAmount);

            _me = playerGameSession.Player;
            UpdateGameSession(playerGameSession);

            TradeVM = new TradeVM(_me);
            TradeVM.TradePlaceRequested += (t) => _me.PlayerController.PlaceTrade(t);

            AuctionVM = new AuctionVM(_playerGameSession.PlayerData.AuctionPayments);
            AuctionVM.UpdatePaymentsCalled += (p) => { _me.PlayerController.UpdateAuctionPayments(p); };

            UpgradeStorageCommand = new RelayCommand(() => { _me.PlayerController.UpgradeStorage(); },
                () => PlayerData.Storage.UpgradeCost <= PlayerData.Money);
        }

        private void SubscribeEvents()
        {
            _me.PlayerControllerCallback.AuctionDoneCalled += PlayerControllerCallback_AuctionDoneCalled;
            _me.PlayerControllerCallback.SuccessCalled += PlayerControllerCallback_SuccessCalled;
            _me.PlayerControllerCallback.FailureCalled += PlayerControllerCallback_FailureCalled;
            _me.PlayerControllerCallback.NewOrderAppearedCalled += PlayerControllerCallback_NewOrderAppearedCalled;
            _me.PlayerControllerCallback.PlacedTradeAcceptedCalled += PlayerControllerCallback_PlacedTradeAcceptedCalled;
            _me.PlayerControllerCallback.PlacedTradeReturnedCalled += PlayerControllerCallback_PlacedTradeReturnedCalled;
            _me.PlayerControllerCallback.GameSessionUpdatedCalled += PlayerControllerCallback_GameSessionUpdatedCalled;
            _me.PlayerControllerCallback.PlayerWonCalled += PlayerControllerCallback_PlayerWonCalled;

            _orders = _playerGameSession.Orders.Select(s => new OrderVM(s, PlayerData)).ToList();
            foreach (var orderVm in _orders)
            {
                orderVm.AcceptOrderCalled += OrderVm_AcceptOrderCalled;
            }

            _trades = _playerGameSession.Trades.Select(s => new TradeAsItemVm(s, PlayerData)).ToList();
            foreach (var tradeAsItemVm in _trades)
            {
                tradeAsItemVm.AcceptTradeCalled += TradeAsItemVm_AcceptTradeCalled;
            }
        }

        private void PlayerControllerCallback_PlayerWonCalled(Player obj)
        {
            MessageBox.Show($"Конец игры! Победил игрок {obj.Name}. Поздравляем!");
        }

        private void TradeAsItemVm_AcceptTradeCalled(Trade obj)
        {
            _me.PlayerController.AcceptTrade(obj);
        }

        private void OrderVm_AcceptOrderCalled(Order obj)
        {
            _me.PlayerController.AcceptOrder(obj);
        }

        private void PlayerControllerCallback_GameSessionUpdatedCalled(PlayerGameSession obj)
        {
            obj.Player = _me;
            UpdateGameSession(obj);
        }

        private void UnsubscribeEvents()
        {
            _me.PlayerControllerCallback.AuctionDoneCalled -= PlayerControllerCallback_AuctionDoneCalled;
            _me.PlayerControllerCallback.SuccessCalled -= PlayerControllerCallback_SuccessCalled;
            _me.PlayerControllerCallback.FailureCalled -= PlayerControllerCallback_FailureCalled;
            _me.PlayerControllerCallback.NewOrderAppearedCalled -= PlayerControllerCallback_NewOrderAppearedCalled;
            _me.PlayerControllerCallback.PlacedTradeAcceptedCalled -= PlayerControllerCallback_PlacedTradeAcceptedCalled;
            _me.PlayerControllerCallback.PlacedTradeReturnedCalled -= PlayerControllerCallback_PlacedTradeReturnedCalled;
            _me.PlayerControllerCallback.GameSessionUpdatedCalled -= PlayerControllerCallback_GameSessionUpdatedCalled;
            _me.PlayerControllerCallback.PlayerWonCalled -= PlayerControllerCallback_PlayerWonCalled;
            
            foreach (var orderVm in _orders)
            {
                orderVm.AcceptOrderCalled -= OrderVm_AcceptOrderCalled;
            }
            
            foreach (var tradeAsItemVm in _trades)
            {
                tradeAsItemVm.AcceptTradeCalled -= TradeAsItemVm_AcceptTradeCalled;
            }
        }

        private void PlayerControllerCallback_PlacedTradeReturnedCalled(Trade obj)
        {
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick,
                "Ваша сделка была отменена."));
        }

        private void PlayerControllerCallback_PlacedTradeAcceptedCalled(Trade obj)
        {
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick,
                "Ваша сделка была успешно проведена."));
        }

        private void PlayerControllerCallback_NewOrderAppearedCalled(Order obj)
        {
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick,
                "Появился новый заказ."));
        }

        private void PlayerControllerCallback_FailureCalled(FailureMessage obj)
        {
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick,
                $"Ошибка: {StringMessage.From(obj)}."));
        }

        private void PlayerControllerCallback_SuccessCalled()
        {
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick, "Успешно."));
        }

        private void PlayerControllerCallback_AuctionDoneCalled(Dictionary<Resource, int> obj)
        {
            AuctionVM.Clear();
            OnPropertyChanged(nameof(AuctionVM));
            LogsAdd(new KeyValuePair<int, string>(_playerGameSession.CurrentTick, "Был проведен аукцион."));
        }

        private void LogsAdd(KeyValuePair<int, string> item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Add(item);
            });
        }

        private void UpdateGameSession(PlayerGameSession playerGameSession)
        {
            if (_playerGameSession != null)
                UnsubscribeEvents();

            _playerGameSession = playerGameSession;
            _me = playerGameSession.Player;
            SubscribeEvents();

            OnPropertyChanged(nameof(PlayerData));
            OnPropertyChanged(nameof(Orders));
            OnPropertyChanged(nameof(Trades));
            OnPropertyChanged(nameof(CurrentTick));
            OnPropertyChanged(nameof(TimeToAuction));
            OnPropertyChanged(nameof(AuctionAmounts));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}