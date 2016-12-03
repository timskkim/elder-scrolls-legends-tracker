﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.ComponentModel;
using ESLTracker.Properties;
using ESLTracker.DataModel.Enums;
using ESLTrackerTests;
using System.Reflection;

namespace ESLTracker.ViewModels.Game.Tests
{
    [TestClass()]
    public class EditGameViewModelTests : BaseTest
    {
        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest001_SaveSettingForCurrentPlayerRank()
        {
            Mock<ISettings> settingsMock = new Mock<ISettings>();

            EditGameViewModel model = new EditGameViewModel();

            PlayerRank selectedPlayerRank = PlayerRank.TheLord;
            model.Game.Type = GameType.PlayRanked;
            model.Game.OpponentClass = DeckClass.Crusader;
            model.Game.PlayerRank = selectedPlayerRank;

            object param = "Victory";

            model.CommandButtonCreateExecute(param, settingsMock.Object);

            settingsMock.VerifySet(s => s.PlayerRank = selectedPlayerRank, Times.Once);

        }

        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest002_RankedFieldsNullForNonRankedGame()
        {
            Mock<ISettings> settingsMock = new Mock<ISettings>();

            EditGameViewModel model = new EditGameViewModel();

            string param = "Victory";

            PopulateObject(model.Game, StartProp);

            Assert.IsNotNull(model.Game.PlayerLegendRank);
            Assert.IsNotNull(model.Game.PlayerRank);
            Assert.IsNotNull(model.Game.OpponentLegendRank);
            Assert.IsNotNull(model.Game.OpponentRank);
            Assert.IsNotNull(model.Game.BonusRound);

            //set type to other than ranked value
            model.Game.Type = GameType.SoloArena;

            model.CommandButtonCreateExecute(param, settingsMock.Object);

            Assert.IsNull(model.Game.PlayerLegendRank);
            Assert.IsNull(model.Game.PlayerRank);
            Assert.IsNull(model.Game.OpponentLegendRank);
            Assert.IsNull(model.Game.OpponentRank);
            Assert.IsNull(model.Game.BonusRound);
        }

        [TestMethod()]
        public void CommandButtonSaveChangesExecuteTest001_RankedFieldsNullForNonRankedGame()
        {
            Mock<ISettings> settingsMock = new Mock<ISettings>();

            EditGameViewModel model = new EditGameViewModel();

            PopulateObject(model.Game, StartProp);

            Assert.IsNotNull(model.Game.PlayerLegendRank);
            Assert.IsNotNull(model.Game.PlayerRank);
            Assert.IsNotNull(model.Game.OpponentLegendRank);
            Assert.IsNotNull(model.Game.OpponentRank);
            Assert.IsNotNull(model.Game.BonusRound);

            //set type to other than ranked value
            model.Game.Type = GameType.SoloArena;

            model.CommandButtonSaveChangesExecute(null);

            Assert.IsNull(model.Game.PlayerLegendRank);
            Assert.IsNull(model.Game.PlayerRank);
            Assert.IsNull(model.Game.OpponentLegendRank);
            Assert.IsNull(model.Game.OpponentRank);
            Assert.IsNull(model.Game.BonusRound);
        }


        [TestMethod()]
        public void SummaryText001_ChangeEventRaisedWhenNameChanges()
        {
            EditGameViewModel model = new EditGameViewModel();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; }; 

            model.Game.OpponentName = "some name";

            Assert.AreEqual(true, raised);
        }


        [TestMethod()]
        public void SummaryText002_ChangeEventRaisedWhenOpponentClassChanges()
        {
            EditGameViewModel model = new EditGameViewModel();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; };

            model.Game.OpponentClass = DeckClass.Endurance;

            Assert.AreEqual(true, raised);
        }

        [TestMethod]
        public void IEditableObjectImplementation001_CancelEdit()
        {
            Mock<IDeckClassSelectorViewModel> deckClassSelector = new Mock<IDeckClassSelectorViewModel>();

            EditGameViewModel model = new EditGameViewModel();
            DataModel.Game game = new DataModel.Game();

            model.Game = game;

            model.Game.ClearEventInvocations("PropertyChanged");

            PopulateObject(game, StartProp);

            TestContext.WriteLine("Begin Edit");
            model.BeginEdit();

            PopulateObject(game, EditProp);

            TestContext.WriteLine("Cancel Edit");
            model.CancelEdit();

            foreach (PropertyInfo p in game.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    if (p.Name == "DeckId")
                    {
                        //deck id is handled different way, depends on Deck
                        DataModel.Deck staringDeck = (DataModel.Deck)StartProp[typeof(DataModel.Deck)];
                        Assert.AreEqual(staringDeck.DeckId, p.GetValue(game), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                    else
                    {
                        Assert.AreEqual(StartProp[p.PropertyType], p.GetValue(game), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                }
            }

        }


    }
}