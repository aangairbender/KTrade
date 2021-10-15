using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class PlayerData
    {
        [DataMember]
        public Storage Storage { get; private set; }

        [DataMember]
        public Dictionary<Resource, int> AuctionPayments { get; private set; }

        [DataMember]
        public int Money { get; set; }

        public static PlayerData Create(GameConfig gameConfig)
        {
            var storage = Storage.Create(gameConfig);
            var auctionPayments = new Dictionary<Resource, int>();
            for (var i = 0; i < Resource.TotalAmount; ++i)
                auctionPayments.Add(Resource.From(i), 0);

            return new PlayerData
            {
                Storage = storage,
                AuctionPayments = auctionPayments,
                Money = gameConfig.StartingMoney
            };
        }
    }
}