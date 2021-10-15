using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KTrade.Core
{
    [ServiceContract(CallbackContract = typeof(IPlayerControllerCallback))]
    public interface IPlayerController
    {
        event Action<Player> UpgradeStorageCalled;
        event Action<Player, Dictionary<Resource, int>> UpdateAuctionPaymentsCalled;
        event Action<Player, Order> AcceptOrderCalled;
        event Action<Player, Trade> AcceptTradeCalled;
        event Action<Player, Trade> PlaceTradeCalled;
        event Action<Player, string> HelloCalled;

        [OperationContract]
        void UpgradeStorage();

        [OperationContract]
        void UpdateAuctionPayments(Dictionary<Resource, int> payments);

        [OperationContract]
        void AcceptOrder(Order order);

        [OperationContract]
        void AcceptTrade(Trade trade);

        [OperationContract]
        void PlaceTrade(Trade trade);

        [OperationContract]
        void Hello(string name);
    }
}