using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class PlayerGameSession
    {
        [DataMember]
        public Player Player { get; set; }

        [DataMember]
        public PlayerData PlayerData { get; set; }

        [DataMember]
        public List<Order> Orders { get; set; }

        [DataMember]
        public List<Trade> Trades { get; set; }

        [DataMember]
        public GameConfig GameConfig { get; set; }

        [DataMember]
        public int CurrentTick { get; set; }

        [DataMember]
        public Dictionary<Resource, int> AuctionAmounts { get; set; }
    }
}