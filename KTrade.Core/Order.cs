using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();

        [DataMember]
        public Dictionary<Resource, int> Resources { get; private set; }

        [DataMember]
        public int Cost { get; private set; }

        public static Order Create(GameConfig config, int tick)
        {
            var resources = new Dictionary<Resource, int>();
            var resourcesTotalAmount = 0;
            var resourcesValue = 0;
            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resource = Resource.From(i);
                var isPresent = config.Random.NextDouble() < 0.5 / resource.RarityTickDivisor;
                if (!isPresent)
                    continue;

                var amount = config.Random.Next(1,
                    Math.Max(1, (int) (tick * config.ResourceOrderFromTickMultiplier / resource.RarityTickDivisor)));
                resources.Add(resource, amount);
                resourcesTotalAmount += amount;

                resourcesValue += amount * resource.RarityTickDivisor * 2;
            }

            if (resourcesTotalAmount == 0)
            {
                //recreating order
                return Create(config, tick);
            }


            var urgency = config.Random.NextDouble();

            var cost = (int) (resourcesValue * (urgency + 0.5));

            return new Order
            {
                Resources = resources,
                Cost = cost
            };
        }
    }
}