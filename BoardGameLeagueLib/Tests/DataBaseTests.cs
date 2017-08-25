using BoardGameLeagueLib.DbClasses;
using log4net;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace BoardGameLeagueLib.Tests
{
    [TestFixture]
    class DataBaseTests
    {
        [Test]
        public void RemoveEntities()
        {
            DbHelper v_DbHelper = DbHelper.Instance;
            ILog v_Logger = LogManager.GetLogger("RemoveEntities");

            // Create some entries
            GameFamily v_TempFamily = new GameFamily("TestGameFamily1");
            v_DbHelper.LiveBglDb.GameFamilies.Add(v_TempFamily);
            v_DbHelper.LiveBglDb.Games.Add(new Game("TestGame01", 2, 4, DbClasses.Game.GameType.VictoryPoints, v_TempFamily.Id));
            v_DbHelper.LiveBglDb.Games.Add(new Game("TestGame02", 2, 4, DbClasses.Game.GameType.VictoryPoints, v_TempFamily.Id));

            // Try to remove the referenced GameFamily.
            BglDb.EntityStatus v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_TempFamily);
            Assert.AreEqual(BglDb.EntityStatus.NotRemoved, v_ActualEntitiyStatus);

            // Create another GameFamily but don't reference it.
            v_TempFamily = new GameFamily("TestGameFamily2");
            v_DbHelper.LiveBglDb.GameFamilies.Add(v_TempFamily);

            // By now we expect two GameFamily items.
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.GameFamilies.Count);
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.GameFamiliesById.Count);

            // Remove the unreferenced GameFamily.
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_TempFamily);
            Assert.AreEqual(BglDb.EntityStatus.Removed, v_ActualEntitiyStatus);

            // There must only be the referenced GameFamily.
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.GameFamilies.Count);
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.GameFamiliesById.Count);

            // Add some new Players.
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.AddEntity(new Player("P. Layer 01", "p1", Player.Genders.Male));
            Assert.AreEqual(BglDb.EntityStatus.Added, v_ActualEntitiyStatus);
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.AddEntity(new Player("P. Layer 02", "p2", Player.Genders.Male));
            Assert.AreEqual(BglDb.EntityStatus.Added, v_ActualEntitiyStatus);
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.AddEntity(new Player("P. Layer 03", "p3", Player.Genders.Male));
            Assert.AreEqual(BglDb.EntityStatus.Added, v_ActualEntitiyStatus);
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.AddEntity(new Player("P. Layer 04", "p4", Player.Genders.Male));
            Assert.AreEqual(BglDb.EntityStatus.Added, v_ActualEntitiyStatus);
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.AddEntity(new Player("P. Layer 05", "p5", Player.Genders.Male));
            Assert.AreEqual(BglDb.EntityStatus.Added, v_ActualEntitiyStatus);

            Assert.AreEqual(5, v_DbHelper.LiveBglDb.Players.Count);
            Assert.AreEqual(5, v_DbHelper.LiveBglDb.PlayersById.Count);

            ObservableCollection<Score> v_Scores = new ObservableCollection<Score>
            {
                {new Score(v_DbHelper.LiveBglDb.Players[0].Id, "10") },
                {new Score(v_DbHelper.LiveBglDb.Players[1].Id, "8") },
                {new Score(v_DbHelper.LiveBglDb.Players[2].Id, "7") },
                {new Score(v_DbHelper.LiveBglDb.Players[3].Id, "4") }
            };

            ObservableCollection<Guid> v_Winners = new ObservableCollection<Guid>
            {
                { v_DbHelper.LiveBglDb.Players[0].Id }
            };

            Assert.AreEqual(0, v_DbHelper.LiveBglDb.Locations.Count);
            Location v_Location1 = new Location("Test Location 01");
            v_DbHelper.LiveBglDb.Locations.Add(v_Location1);
            Location v_Location2 = new Location("Test Location 02");
            v_DbHelper.LiveBglDb.Locations.Add(v_Location2);
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.Locations.Count);
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.LocationsById.Count);

            v_DbHelper.LiveBglDb.Results.Add(new Result(v_DbHelper.LiveBglDb.Games[0].Id, v_Scores, v_Winners, new DateTime(2017, 08, 22), v_Location2.Id));

            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_DbHelper.LiveBglDb.Players[0]);
            Assert.AreEqual(BglDb.EntityStatus.NotRemoved, v_ActualEntitiyStatus);

            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_DbHelper.LiveBglDb.Players[1]);
            Assert.AreEqual(BglDb.EntityStatus.NotRemoved, v_ActualEntitiyStatus);

            Assert.AreEqual(5, v_DbHelper.LiveBglDb.Players.Count);
            Assert.AreEqual(5, v_DbHelper.LiveBglDb.PlayersById.Count);
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_DbHelper.LiveBglDb.Players[4]);
            Assert.AreEqual(BglDb.EntityStatus.Removed, v_ActualEntitiyStatus);
            Assert.AreEqual(4, v_DbHelper.LiveBglDb.Players.Count);
            Assert.AreEqual(4, v_DbHelper.LiveBglDb.PlayersById.Count);

            v_Logger.Info(String.Format("Removing [{0}], expecting [{1}]", v_DbHelper.LiveBglDb.Games[0].Name, BglDb.EntityStatus.NotRemoved));
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_DbHelper.LiveBglDb.Games[0]);
            Assert.AreEqual(BglDb.EntityStatus.NotRemoved, v_ActualEntitiyStatus);

            Assert.AreEqual(2, v_DbHelper.LiveBglDb.Games.Count);
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.GamesById.Count);
            v_Logger.Info(String.Format("Removing [{0}], expecting [{1}]", v_DbHelper.LiveBglDb.Games[1].Name, BglDb.EntityStatus.Removed));
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_DbHelper.LiveBglDb.Games[1]);
            Assert.AreEqual(BglDb.EntityStatus.Removed, v_ActualEntitiyStatus);
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.Games.Count);
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.GamesById.Count);

            v_Logger.Info(String.Format("Removing [{0}], expecting [{1}]", v_Location2.Name, BglDb.EntityStatus.NotRemoved));
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_Location2);
            Assert.AreEqual(BglDb.EntityStatus.NotRemoved, v_ActualEntitiyStatus);

            Assert.AreEqual(2, v_DbHelper.LiveBglDb.Locations.Count);
            Assert.AreEqual(2, v_DbHelper.LiveBglDb.LocationsById.Count);
            v_Logger.Info(String.Format("Removing [{0}], expecting [{1}]", v_Location1.Name, BglDb.EntityStatus.NotRemoved));
            v_ActualEntitiyStatus = v_DbHelper.LiveBglDb.RemoveEntity(v_Location1);
            Assert.AreEqual(BglDb.EntityStatus.Removed, v_ActualEntitiyStatus);
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.Locations.Count);
            Assert.AreEqual(1, v_DbHelper.LiveBglDb.LocationsById.Count);
        }
    }
}
