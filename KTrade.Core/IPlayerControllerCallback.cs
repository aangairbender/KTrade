using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KTrade.Core
{
    [ServiceContract]
    public interface IPlayerControllerCallback
    {
        event Action SuccessCalled;
        event Action<FailureMessage> FailureCalled;
        event Action<Trade> PlacedTradeAcceptedCalled;
        event Action<Trade> PlacedTradeReturnedCalled;
        event Action<Order> NewOrderAppearedCalled;
        event Action<PlayerGameSession> GameSessionUpdatedCalled;
        event Action<Dictionary<Resource, int>> AuctionDoneCalled;
        event Action<Player> PlayerWonCalled;

        [OperationContract(IsOneWay = true)]
        void Success();

        [OperationContract(IsOneWay = true)]
        void Failure(FailureMessage message);

        [OperationContract(IsOneWay = true)]
        void PlacedTradeAccepted(Trade trade);

        [OperationContract(IsOneWay = true)]
        void PlacedTradeReturned(Trade trade);

        [OperationContract(IsOneWay = true)]
        void NewOrderAppeared(Order order);

        [OperationContract(IsOneWay = true)]
        void GameSessionUpdated(PlayerGameSession playerGameSession);

        [OperationContract(IsOneWay = true)]
        void AuctionDone(Dictionary<Resource, int> income);

        [OperationContract(IsOneWay = true)]
        void PlayerWon(Player player);
    }
}