using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using KTrade.Client.Annotations;
using KTrade.Client.Common;
using KTrade.Core;

namespace KTrade.Client.VM
{
    public class AuctionVM : INotifyPropertyChanged
    {
        public Dictionary<Resource, IntWrapper> MyAuctionPayments { get; }

        public event Action<Dictionary<Resource, int>> UpdatePaymentsCalled;

        public RelayCommand UpdatePayments { get; }

        public AuctionVM(Dictionary<Resource, int> baseValues)
        {
            MyAuctionPayments = new Dictionary<Resource, IntWrapper>();
            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resourse = Resource.From(i);
                MyAuctionPayments.Add(resourse, new IntWrapper { Value = baseValues[resourse] });
            }

            UpdatePayments = new RelayCommand(() =>
            {
                if (!Validate())
                    return;
                    UpdatePaymentsCalled?.Invoke(MyAuctionPayments.ToDictionary(kvp => kvp.Key,
                        kvp => kvp.Value.Value));
            });
        }

        private bool Validate()
        {
            foreach (var kvp in MyAuctionPayments)
            {
                var input = kvp.Value.Value;
                if (input < 0)
                {
                    MessageBox.Show("Количество денег не может быть отрицательным");
                    return false;
                }
            }

            return true;
        }

        public event Action Cleared;

        public void Clear()
        {
            foreach (var resource in MyAuctionPayments.Keys)
            {
                MyAuctionPayments[resource].Value = 0;
            }
            Cleared?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}