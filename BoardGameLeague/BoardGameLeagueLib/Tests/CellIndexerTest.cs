using NUnit.Framework;

namespace BoardGameLeagueUI.BoardGameLeagueLib.Tests
{
    [TestFixture]
    class CellIndexerTest
    {
        [Test]
        public void IndexerTest()
        {
            CellIndexer v_Indexer = CellIndexer.Instance;
            v_Indexer.Reset(10,2);

            Assert.AreEqual(0, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(1, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            Assert.AreEqual(2, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(3, v_Indexer.ActualIndex);
            Assert.AreEqual(3, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            Assert.AreEqual(4, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(5, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            Assert.AreEqual(6, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(7, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            Assert.AreEqual(8, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(9, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            Assert.AreEqual(0, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

            Assert.AreEqual(1, v_Indexer.ActualIndex);
            Assert.AreEqual(1, v_Indexer.RowIndex);

            v_Indexer.Reset(1, 0);
            Assert.AreEqual(0, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);
            Assert.AreEqual(0, v_Indexer.ActualIndex);
            Assert.AreEqual(0, v_Indexer.RowIndex);

        }
    }
}
