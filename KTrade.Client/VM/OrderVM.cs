using System;
using System.Collections.Generic;
using KTrade.Client.Common;
using KTrade.Core;

namespace KTrade.Client.VM
{
    public class OrderVM
    {
        private readonly Order _order;
        private readonly PlayerData _playerData;

        public Dictionary<Resource, int> Resources => _order.Resources;
        public int Cost => _order.Cost;

        public RelayCommand AcceptOrder { get; }

        public event Action<Order> AcceptOrderCalled;

        public OrderVM(Order order, PlayerData playerData)
        {
            _order = order;
            _playerData = playerData;
            AcceptOrder = new RelayCommand(() => { AcceptOrderCalled?.Invoke(_order); }, () =>
            {
                foreach (var kvp in _order.Resources)
                {
                    var resource = kvp.Key;
                    var amount = kvp.Value;
                    if (playerData.Storage.CurrentAmounts[resource] < amount)
                        return false;
                }

                return true;
            });
        }
    }
}