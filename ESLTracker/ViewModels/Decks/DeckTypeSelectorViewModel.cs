﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel.Enums;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckTypeSelectorViewModel : ViewModelBase, IDeckTypeSelectorViewModel
    {
        /// <summary>
        /// fiter of attributes, 
        /// bool value binded to isenabled property of trigger button
        /// </summary>
        public Dictionary<DeckType, bool> FilterButtonState { get; set; }


        public ObservableCollection<DeckType> FilteredTypes { get; set; } = new ObservableCollection<DeckType>(Enum.GetValues(typeof(DeckType)).OfType<DeckType>());

        private bool showCompletedArenaRuns;
        public bool ShowCompletedArenaRuns
        {
            get { return showCompletedArenaRuns; }
            set {
                showCompletedArenaRuns = value;
                RaisePropertyChangedEvent(nameof(ShowCompletedArenaRuns));
                messanger.Send(
                    new DeckListFilterChanged(DeckListFilterChanged.Source.TypeFilter, FilteredTypes, showCompletedArenaRuns, showHiddenDecks, null, null),
                    ControlMessangerContext.DeckList_DeckFilterControl);
            }
        }

        private bool showHiddenDecks;
        public bool ShowHiddenDecks
        {
            get { return showHiddenDecks; }
            set
            {
                showHiddenDecks = value;
                RaisePropertyChangedEvent(nameof(ShowHiddenDecks));
                messanger.Send(
                    new DeckListFilterChanged(DeckListFilterChanged.Source.TypeFilter, FilteredTypes, showCompletedArenaRuns, showHiddenDecks, null, null),
                    ControlMessangerContext.DeckList_DeckFilterControl);
            }
        }

        IMessenger messanger;
        ITrackerFactory trackerFactory;


        //command for filter toggle button pressed
        public ICommand CommandFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(FilterClicked)); }
        }

        public DeckTypeSelectorViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {
        }

        public DeckTypeSelectorViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            messanger = trackerFactory.GetService<IMessenger>();
            FilterButtonState = new Dictionary<DeckType, bool>();
            foreach (DeckType a in Enum.GetValues(typeof(DeckType)))
            {
                FilterButtonState.Add(a, false);
            }
            messanger.Register<DeckListResetFilters>(this, ResetFilter, ControlMessangerContext.DeckList_DeckFilterControl);

            //init filters (so checkbox for hiding completed arena runs is applied
            messanger.Send(new DeckListFilterChanged(DeckListFilterChanged.Source.TypeFilter, FilteredTypes, showCompletedArenaRuns, showHiddenDecks, null, null), ControlMessangerContext.DeckList_DeckFilterControl);
        }

        public void FilterClicked(object param)
        {
            DeckType attrib;
            if (!Enum.TryParse<DeckType>(param.ToString(), out attrib))
            {
                throw new ArgumentException(string.Format("Unknow value for DeckType={0}", param));
            }

            //toggle filter value
            FilterButtonState[attrib] = !FilterButtonState[attrib];

            //fires to many events on colection, will filter once for reset, and then for every selected attrib
            FilteredTypes.Clear();

            //if all unslected - show all
            if (FilterButtonState.All(f => !f.Value))
            {
                Enum.GetValues(typeof(DeckType)).OfType<DeckType>().All(t => { FilteredTypes.Add(t); return true; });
            }
            else
            {
                foreach (DeckType type in FilterButtonState.Where(f => f.Value).Select(f => f.Key))
                {
                    FilteredTypes.Add(type);
                }
            }

            messanger.Send(
                new DeckListFilterChanged(DeckListFilterChanged.Source.TypeFilter, FilteredTypes, showCompletedArenaRuns, showHiddenDecks, null, null),
                ControlMessangerContext.DeckList_DeckFilterControl);

        }

        public void Reset()
        {
            foreach (DeckType a in Enum.GetValues(typeof(DeckType)))
            {
                //unselct all in UI
                FilterButtonState[a] = false;

                //reset filter collection
                FilteredTypes.Clear();
                Enum.GetValues(typeof(DeckType)).OfType<DeckType>().All(t => { FilteredTypes.Add(t); return true; });
            }
            ShowCompletedArenaRuns = false;
            ShowHiddenDecks = false;
            RaisePropertyChangedEvent("FilterButtonState");
        }

        private void ResetFilter(DeckListResetFilters obj)
        {
            Reset();
        }
    }
}
