using System;
using System.Collections.Generic;
using KTrade.Client.Common;
using KTrade.Core;

namespace KTrade.Client.VM
{
    public class TradeAsItemVm
    {
        private readonly Trade _trade;
        private readonly PlayerData _playerData;

        public Player Author => _trade.Author;
        public Dictionary<Resource, int> GivenResources => _trade.GivenResources;
        public Dictionary<Resource, int> AskingResources => _trade.AskingResources;
        public int TimeLeft => _trade.TimeLeft;
        public int GivingMoney => _trade.GivingMoney;
        public int AskingMoney => _trade.AskingMoney;

        public RelayCommand AcceptTrade { get; }

        public event Action<Trade> AcceptTradeCalled;

        public TradeAsItemVm(Trade trade, PlayerData playerData)
        {
            _trade = trade;
            _playerData = playerData;

            AcceptTrade = new RelayCommand(() => { AcceptTradeCalled?.Invoke(_trade); }, () =>
            {
                if (_trade.AskingMoney > _playerData.Money)
                    return false;
                foreach (var kvp in AskingResources)
                {
                    var resource = kvp.Key;
                    var amount = kvp.Value;
                    if (amount > playerData.Storage.CurrentAmounts[resource])
                        return false;
                }

                return true;
            });
        }
    }
}