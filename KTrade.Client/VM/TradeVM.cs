using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KTrade.Client.Common;
using KTrade.Core;

namespace KTrade.Client.VM
{
    public class TradeVM
    {
        private readonly Player _author;

        public event Action<Trade> TradePlaceRequested;

        public Dictionary<Resource, IntWrapper> GivenResources { get; set; }
        public Dictionary<Resource, IntWrapper> AskingResources { get; set; }

        public int TimeLeft { get; set; } = 180;

        public int GivingMoney { get; set; } = 0;
        public int AskingMoney { get; set; } = 0;

        public RelayCommand PlaceTradeCommand { get; }

        public TradeVM(Player author)
        {
            _author = author;
            GivenResources = new Dictionary<Resource, IntWrapper>();
            AskingResources = new Dictionary<Resource, IntWrapper>();
            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resource = Resource.From(i);
                GivenResources.Add(resource, new IntWrapper {Value = 0});
                AskingResources.Add(resource, new IntWrapper {Value = 0});
            }

            PlaceTradeCommand = new RelayCommand(() =>
            {
                if (!Validate())
                    return;
                var trade = new Trade
                {
                    AskingMoney = AskingMoney,
                    GivingMoney = GivingMoney,
                    Author = _author,
                    TimeLeft = TimeLeft,
                    GivenResources = GivenResources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value),
                    AskingResources = AskingResources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value),
                };
                TradePlaceRequested?.Invoke(trade);
            });
        }

        private bool Validate()
        {
            if (TimeLeft < 30 || TimeLeft > 180)
            {
                MessageBox.Show("Время до отмены может быть от 30 до 180");
                return false;
            }

            if (GivingMoney < 0 || AskingMoney < 0)
            {
                MessageBox.Show("Количетсво денег не может быть отрицательным");
                return false;
            }

            if (GivingMoney != 0 && AskingMoney != 0)
            {
                MessageBox.Show("Хотя бы одно из полей (Даю денег) и (Хочу денег) должно быть нулевым");
                return false;
            }

            foreach (var kvp in GivenResources)
            {
                var input = kvp.Value.Value;
                if (input < 0)
                {
                    MessageBox.Show("Количество ресурса не может быть отрицательным");
                    return false;
                }
            }

            foreach (var kvp in AskingResources)
            {
                var input = kvp.Value.Value;
                if (input < 0)
                {
                    MessageBox.Show("Количество ресурса не может быть отрицательным");
                    return false;
                }
            }

            if (GivenResources.Sum(kvp => kvp.Value.Value) + AskingResources.Sum(kvp => kvp.Value.Value) == 0)
            {
                MessageBox.Show("В сделке должны учавствовать ресурсы");
                return false;
            }

            return true;
        }
    }
}