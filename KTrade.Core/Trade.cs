using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class Trade
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();

        [DataMember]
        public Player Author { get; set; }

        [DataMember]
        public Dictionary<Resource, int> GivenResources { get; set; }

        [DataMember]
        public Dictionary<Resource, int> AskingResources { get; set; }

        [DataMember]
        public int TimeLeft { get; set; }

        public void Tick()
        {
            TimeLeft -= 1;
        }

        [DataMember]
        public int GivingMoney { get; set; }

        [DataMember]
        public int AskingMoney { get; set; }

        public static Trade CreateEmpty(Player author)
        {
            var givenResources = new Dictionary<Resource, int>();
            var askingResources = new Dictionary<Resource, int>();
            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resource = Resource.From(i);
                givenResources.Add(resource, 0);
                askingResources.Add(resource, 0);
            }

            return new Trade
            {
                Author = author,
                GivingMoney = 0,
                AskingMoney = 0,
                TimeLeft = 180,
                GivenResources = givenResources,
                AskingResources = askingResources
            };
        }
    }
}