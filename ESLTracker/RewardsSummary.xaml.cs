﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for RewardsSummary.xaml
    /// </summary>
    public partial class RewardsSummary : Window
    {
        private TrackerFactory trackerFactory;

        public RewardsSummary() : this(new TrackerFactory())
        {
            
        }

        internal RewardsSummary(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            InitializeComponent();
        }

        private void OnTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                // this tab is selected!
                this.rewardsSummary.DataContext = trackerFactory.GetTracker().Rewards
                    .GroupBy(r=> new { r.Type, r.Reason })
                    .Select(rs=> new DataModel.Reward() {
                        Type = rs.Key.Type,
                        Reason = rs.Key.Reason,
                        Quantity = rs.Where(r=> r.Type == rs.Key.Type).Sum(r=> r.Quantity)
                    })
                    .OrderBy(rs=> rs.Type);
            }
        }
    }
}
