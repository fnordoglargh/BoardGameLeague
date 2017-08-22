using BoardGameLeagueLib.DbClasses;
using NUnit.Framework;

namespace BoardGameLeagueLib.Tests
{
    [TestFixture]
    class DataBaseTests
    {
        [Test]
        public void RemoveEntities()
        {
            DbHelper v_DbHelper = DbHelper.Instance;

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
        }
    }
}
