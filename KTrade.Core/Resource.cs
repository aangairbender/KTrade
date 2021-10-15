using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace KTrade.Core
{
    [DataContract]
    public struct Resource
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public int RarityTickDivisor { get; private set; }

        public Brush Brush => ResourceBrushes[Id];

        private static readonly Dictionary<int, Resource> Resources = new Dictionary<int, Resource>();
        public static Resource From(int id)
        {
            return Resources[id];
        }

        public static int TotalAmount { get; private set; }

        public static void SetBaseAmount(int baseAmount)
        {
            var mediumAmount = Math.Max(1, baseAmount / 2);
            var rareAmount = Math.Max(1, mediumAmount / 2);
            TotalAmount = baseAmount + mediumAmount + rareAmount;
            for (var i = 0; i < TotalAmount; ++i)
            {
                if (i < baseAmount)
                    Resources.Add(i, new Resource{Id = i, RarityTickDivisor = 1});
                else if (i < baseAmount + mediumAmount)
                    Resources.Add(i, new Resource {Id = i, RarityTickDivisor = 2});
                else
                    Resources.Add(i, new Resource {Id = i, RarityTickDivisor = 10});
            }
        }

        private static readonly Brush[] ResourceBrushes =
        {
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Yellow,
            Brushes.Purple,
            Brushes.Pink,
            Brushes.Aqua
        };
    }
}