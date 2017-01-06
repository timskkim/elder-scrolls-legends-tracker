﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.DataModel;

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for CardList.xaml
    /// </summary>
    public partial class CardList : UserControl
    {
        public ObservableCollection<CardInstance> CardCollection
        {
            get { return (ObservableCollection<CardInstance>)GetValue(CardCollectionProperty); }
            set { SetValue(CardCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardsList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardCollectionProperty =
            DependencyProperty.Register(nameof(CardCollection), typeof(ObservableCollection<CardInstance>), typeof(CardList), new PropertyMetadata(new ObservableCollection<CardInstance>()));



        public CardList()
        {
            InitializeComponent();
        }
    }
}