using System;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class GameConfig
    {
        [DataMember]
        public int BaseResourcesAmount { get; set; }

        [DataMember]
        public int StartingMoney { get; set; }

        [DataMember]
        public int StorageAmountForLevel { get; set; }

        [DataMember]
        public int StorageCostForLevel { get; set; }

        [DataMember]
        public int AuctionCooldown { get; set; }

        [DataMember]
        public int MoneyForWin { get; set; }

        [DataMember]
        public double ResourceOrderFromTickMultiplier { get; set; }

        [DataMember]
        public double ResourceAuctionFromTickMultiplier { get; set; }

        [DataMember]
        public int BonusInterval { get; set; }

        [DataMember]
        public int BonusAmount { get; set; }

        [DataMember]
        public double OrdersAmountFromPlayerAmountMultiplier { get; set; }

        private int _randomSeed;

        [DataMember]
        public int RandomSeed
        {
            get => _randomSeed;
            set
            {
                _randomSeed = value;
                Random = new Random(value);
            }
        }

        [DataMember]
        public Random Random { get; private set; } = new Random();
    }
}