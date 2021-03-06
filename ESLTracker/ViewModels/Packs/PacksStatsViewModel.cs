﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Packs
{
    public class PacksStatsViewModel : ViewModelBase
    {
        public IEnumerable<Pack> OrderedPacks
        {
            get
            {
                return trackerFactory.GetTracker().Packs.OrderByDescending(p => p.DateOpened);
            }
        }

        public int PacksSinceLegendary
        {
            get
            {
                return PacksSince(c => c?.Card?.Rarity == CardRarity.Legendary);
            }
        }

        public int MaxPacksSinceLegendary
        {
            get
            {
                return MaxPacksSince(c => c.Card.Rarity == CardRarity.Legendary);
            }
        }

        public int PacksSinceEpic
        {
            get
            {
                return PacksSince(c => c?.Card?.Rarity == CardRarity.Epic);
            }
        }

        public int MaxPacksSinceEpic
        {
            get
            {
                return MaxPacksSince(c => c.Card.Rarity == CardRarity.Epic);
            }
        }

        public int PacksSincePremium
        {
            get
            {
                return PacksSince(c => c?.IsPremium == true);
            }
        }

        public int MaxPacksSincePremium
        {
            get
            {
                return MaxPacksSince(c => c?.IsPremium == true);
            }
        }

        public double AveragePackValue
        {
            get
            {
                if (OrderedPacks.Count() > 0)
                {
                    return Math.Round(OrderedPacks.Average(p => p.SoulGemsValue), 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        private TrackerFactory trackerFactory;

        public PacksStatsViewModel() : this(new TrackerFactory())
        {

        }

        public PacksStatsViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.trackerFactory.GetTracker().Packs.CollectionChanged += Packs_CollectionChanged;
        }

        private void Packs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent("");
        }

        private int PacksSince(Func<CardInstance, bool> filter)
        {
            if (OrderedPacks.Count() == 0)
            {
                return 0;
            }
            return OrderedPacks
                .Where(p =>
                    p.DateOpened > OrderedPacks
                                    .Where(p2 => p2.Cards.Any(filter))
                                    .DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue })
                                    .FirstOrDefault()
                                    .DateOpened
                        ).Count();
        }

        private int MaxPacksSince(Func<CardInstance, bool> filter)
        {
            if (OrderedPacks.Count() == 0)
            {
                return 0;
            }

            //indexes of searched packs
            var packIndexes = OrderedPacks.Select((p, index) => new { p, index })
                .Where(col => col.p.Cards.Any(filter))
                .Select(col => col.index);
            if (packIndexes.Count() == 0)
            {
                //nothing fulfills filter - return all!
                return OrderedPacks.Count();
            }
            else if (packIndexes.Count() == 1)
            {
                return packIndexes.First();
            }
            else
            {
                return Math.Max(packIndexes.First(),
                    packIndexes.Zip(packIndexes.Skip(1), (x, y) => y - x).Max()
                    );
            }
        }
    }
}
