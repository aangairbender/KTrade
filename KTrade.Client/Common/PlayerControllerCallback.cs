using System;
using System.Collections.Generic;
using System.ServiceModel;
using KTrade.Core;

namespace KTrade.Client.Common
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PlayerControllerCallback : IPlayerControllerCallback
    {
        public event Action SuccessCalled;
        public event Action<FailureMessage> FailureCalled;
        public event Action<Trade> PlacedTradeAcceptedCalled;
        public event Action<Trade> PlacedTradeReturnedCalled;
        public event Action<Order> NewOrderAppearedCalled;
        public event Action<PlayerGameSession> GameSessionUpdatedCalled;
        public event Action<Dictionary<Resource, int>> AuctionDoneCalled;
        public event Action<Player> PlayerWonCalled;

        public void Success()
        {
            SuccessCalled?.Invoke();
        }

        public void Failure(FailureMessage message)
        {
            FailureCalled?.Invoke(message);
        }

        public void PlacedTradeAccepted(Trade trade)
        {
            PlacedTradeAcceptedCalled?.Invoke(trade);
        }

        public void PlacedTradeReturned(Trade trade)
        {
            PlacedTradeReturnedCalled?.Invoke(trade);
        }

        public void NewOrderAppeared(Order order)
        {
            NewOrderAppearedCalled?.Invoke(order);
        }

        public void GameSessionUpdated(PlayerGameSession playerGameSession)
        {
            GameSessionUpdatedCalled?.Invoke(playerGameSession);
        }

        public void AuctionDone(Dictionary<Resource, int> income)
        {
            AuctionDoneCalled?.Invoke(income);
        }

        public void PlayerWon(Player player)
        {
            PlayerWonCalled?.Invoke(player);
        }
    }
}