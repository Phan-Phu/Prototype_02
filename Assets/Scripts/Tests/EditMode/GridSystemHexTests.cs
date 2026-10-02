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

        [Test]
        public void GetWorldPosition_ThenGetGridPosition_RoundTripsForEveryCell()
        {
            // Same size as the shipped level, so odd/even rows and every edge are covered.
            var grid = new GridSystemHex<int>(15, 20, 2, (g, pos) => 0);

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    var original = new GridPosition(x, y);

                    Assert.AreEqual(original, grid.GetGridPosition(grid.GetWorldPosition(original)));
                }
            }
        }

        [Test]
        public void GetNeighbours_EvenRowInteriorCell_ReturnsSixCellsShiftedLeft()
        {
            var grid = new GridSystemHex<int>(6, 6, 2, (g, pos) => 0);

            var neighbours = grid.GetNeighbours(new GridPosition(3, 2));

            CollectionAssert.AreEquivalent(new[]
            {
                new GridPosition(2, 2), new GridPosition(4, 2),
                new GridPosition(3, 3), new GridPosition(3, 1),
                new GridPosition(2, 3), new GridPosition(2, 1),
            }, neighbours);
        }

        [Test]
        public void GetNeighbours_OddRowInteriorCell_ReturnsSixCellsShiftedRight()
        {
            var grid = new GridSystemHex<int>(6, 6, 2, (g, pos) => 0);

            var neighbours = grid.GetNeighbours(new GridPosition(3, 1));

            CollectionAssert.AreEquivalent(new[]
            {
                new GridPosition(2, 1), new GridPosition(4, 1),
                new GridPosition(3, 2), new GridPosition(3, 0),
                new GridPosition(4, 2), new GridPosition(4, 0),
            }, neighbours);
        }

        [Test]
        public void GetNeighbours_TopRowCell_StaysInsideGrid()
        {
            // Regression: the old A* adapter produced y == height neighbours on the top row.
            var grid = new GridSystemHex<int>(5, 4, 2, (g, pos) => 0);

            for (int x = 0; x < 5; x++)
            {
                foreach (GridPosition neighbour in grid.GetNeighbours(new GridPosition(x, 3)))
                {
                    Assert.IsTrue(grid.IsValidGridPosition(neighbour), $"{neighbour} is outside the grid");
                }
            }
        }

        [Test]
        public void GetDistance_ReturnsHexStepCount()
        {
            var grid = new GridSystemHex<int>(6, 6, 2, (g, pos) => 0);

            Assert.AreEqual(0, grid.GetDistance(new GridPosition(2, 2), new GridPosition(2, 2)));
            Assert.AreEqual(1, grid.GetDistance(new GridPosition(3, 1), new GridPosition(4, 2)));
            Assert.AreEqual(3, grid.GetDistance(new GridPosition(0, 0), new GridPosition(3, 0)));
            Assert.AreEqual(2, grid.GetDistance(new GridPosition(0, 0), new GridPosition(0, 2)));
        }

        [Test]
        public void GetGridPositionsInRange_RangeOne_ReturnsCenterAndNeighbours()
        {
            var grid = new GridSystemHex<int>(6, 6, 2, (g, pos) => 0);
            var center = new GridPosition(3, 2);

            var expected = grid.GetNeighbours(center);
            expected.Add(center);

            CollectionAssert.AreEquivalent(expected, grid.GetGridPositionsInRange(center, 1));
        }

        [Test]
        public void GetGridPositionsInRange_NearCorner_IsClippedToGrid()
        {
            var grid = new GridSystemHex<int>(6, 6, 2, (g, pos) => 0);

            var positions = grid.GetGridPositionsInRange(new GridPosition(0, 0), 2);

            foreach (GridPosition position in positions)
            {
                Assert.IsTrue(grid.IsValidGridPosition(position));
                Assert.LessOrEqual(grid.GetDistance(new GridPosition(0, 0), position), 2);
            }
            CollectionAssert.Contains(positions, new GridPosition(2, 0));
            CollectionAssert.DoesNotContain(positions, new GridPosition(3, 0));
        }
    }
}
