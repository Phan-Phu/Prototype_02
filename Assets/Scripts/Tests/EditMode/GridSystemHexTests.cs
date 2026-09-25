using Domain;
using Infrastructure;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class GridSystemHexTests
    {
        [Test]
        public void Constructor_CreatesGridObjectForEveryCell()
        {
            int callCount = 0;

            var grid = new GridSystemHex<int>(3, 2, 10, (g, pos) =>
            {
                callCount++;
                return pos.x * 10 + pos.y;
            });

            Assert.AreEqual(6, callCount);
            Assert.AreEqual(3, grid.GetWidth());
            Assert.AreEqual(2, grid.GetHeight());
        }

        [Test]
        public void GetGridObject_ReturnsValueFromFactory()
        {
            var grid = new GridSystemHex<string>(2, 2, 10, (g, pos) => pos.ToString());

            var obj = grid.GetGridObject(new GridPosition(1, 0));

            Assert.AreEqual(new GridPosition(1, 0).ToString(), obj);
        }

        [Test]
        public void IsValidGridPosition_InsideBounds_ReturnsTrue()
        {
            var grid = new GridSystemHex<int>(4, 4, 10, (g, pos) => 0);

            Assert.IsTrue(grid.IsValidGridPosition(new GridPosition(0, 0)));
            Assert.IsTrue(grid.IsValidGridPosition(new GridPosition(3, 3)));
        }

        [Test]
        public void IsValidGridPosition_OutsideBounds_ReturnsFalse()
        {
            var grid = new GridSystemHex<int>(4, 4, 10, (g, pos) => 0);

            Assert.IsFalse(grid.IsValidGridPosition(new GridPosition(-1, 0)));
            Assert.IsFalse(grid.IsValidGridPosition(new GridPosition(0, 4)));
            Assert.IsFalse(grid.IsValidGridPosition(new GridPosition(4, 0)));
        }

        [Test]
        public void GetWorldPosition_ThenGetGridPosition_RoundTrips()
        {
            var grid = new GridSystemHex<int>(5, 5, 10, (g, pos) => 0);
            var original = new GridPosition(2, 2);

            var worldPos = grid.GetWorldPosition(original);
            var roundTripped = grid.GetGridPosition(worldPos);

            Assert.AreEqual(original, roundTripped);
        }
    }
}
