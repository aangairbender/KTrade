using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KTrade.Core
{
    [DataContract]
    public class Storage
    {
        [DataMember]
        public GameConfig GameConfig { get; private set; }

        [DataMember]
        public int Level { get; private set; }

        [DataMember]
        public Dictionary<Resource, int> CurrentAmounts { get; private set; }
        
        public int MaximumAmount => Level * GameConfig.StorageAmountForLevel;
        public int UpgradeCost => Level * GameConfig.StorageCostForLevel;

        public int CurrentTotalAmount => CurrentAmounts.Sum(kvp => kvp.Value);

        public void Upgrade()
        {
            Level += 1;
        }

        public void Add(Resource resource, int amount)
        {
            var toTheLimit = MaximumAmount - CurrentTotalAmount;
            if (amount > toTheLimit)
                amount = toTheLimit;

            CurrentAmounts[resource] += amount;
        }

        public void Remove(Resource resource, int amount)
        {
            CurrentAmounts[resource] -= amount;
        }

        public static Storage Create(GameConfig gameConfig)
        {
            var currentAmounts = new Dictionary<Resource, int>();
            for (var i = 0; i < Resource.TotalAmount; ++i)
                currentAmounts.Add(Resource.From(i), 0);

            return new Storage
            {
                CurrentAmounts = currentAmounts,
                Level = 1,
                GameConfig = gameConfig
            };
        }
    }
}