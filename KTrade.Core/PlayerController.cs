using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KTrade.Core
{
    public class PlayerController : IPlayerController
    {
        private readonly Player _player;

        public static event Action<Player> PlayerConnected;

        private static readonly PlayerNameGenerator NameGenerator = new PlayerNameGenerator();

        public PlayerController()
        {
            _player = new Player()
            {
                Name = NameGenerator.NextName(),
                PlayerController = this,
                PlayerControllerCallback = OperationContext.Current.GetCallbackChannel<IPlayerControllerCallback>()
            };

            PlayerConnected?.Invoke(_player);
        }

        public event Action<Player> UpgradeStorageCalled;
        public event Action<Player, Dictionary<Resource, int>> UpdateAuctionPaymentsCalled;
        public event Action<Player, Order> AcceptOrderCalled;
        public event Action<Player, Trade> AcceptTradeCalled;
        public event Action<Player, Trade> PlaceTradeCalled;
        public event Action<Player, string> HelloCalled;

        public void UpgradeStorage()
        {
            UpgradeStorageCalled?.Invoke(_player);
        }

        public void UpdateAuctionPayments(Dictionary<Resource, int> payments)
        {
            UpdateAuctionPaymentsCalled?.Invoke(_player, payments);
        }

        public void AcceptOrder(Order order)
        {
            AcceptOrderCalled?.Invoke(_player, order);
        }

        public void AcceptTrade(Trade trade)
        {
            AcceptTradeCalled?.Invoke(_player, trade);
        }

        public void PlaceTrade(Trade trade)
        {
            PlaceTradeCalled?.Invoke(_player, trade);
        }

        public void Hello(string name)
        {
            if (_player.Name.Length > 0)
                _player.Name = name;

            HelloCalled?.Invoke(_player, name);
        }
    }
}