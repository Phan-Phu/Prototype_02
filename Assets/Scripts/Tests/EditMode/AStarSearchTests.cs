using System;
using System.Collections.Generic;
using System.Linq;
using AStarPathfinding;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class AStarSearchTests
    {
        // Minimal in-memory square grid used only to exercise AStarSearch<T> without any
        // Unity/game dependency - AStarSearch itself does not know about hex grids.
        private class FakeGrid : IAStarGrid<(int, int)>
        {
            private readonly int width;
            private readonly int height;
            private readonly HashSet<(int, int)> blocked;

            public FakeGrid(int width, int height, IEnumerable<(int, int)> blocked = null)
            {
                this.width = width;
                this.height = height;
                this.blocked = blocked != null ? new HashSet<(int, int)>(blocked) : new HashSet<(int, int)>();
            }

            public bool IsWalkable((int, int) position)
            {
                return !blocked.Contains(position);
            }

            public IEnumerable<(int, int)> GetNeighbours((int, int) position)
            {
                int x = position.Item1;
                int y = position.Item2;
                var candidates = new (int, int)[]
                {
                    (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)
                };
                return candidates.Where(p => p.Item1 >= 0 && p.Item1 < width && p.Item2 >= 0 && p.Item2 < height);
            }

            public int GetMoveCost((int, int) from, (int, int) to) => 1;

            public int GetHeuristicCost((int, int) from, (int, int) to)
            {
                return Math.Abs(from.Item1 - to.Item1) + Math.Abs(from.Item2 - to.Item2);
            }

            public void OnNodeCostsUpdated((int, int) position, int gCost, int hCost, int fCost)
            {
            }
        }

        [Test]
        public void FindPath_StraightLine_ReturnsShortestPath()
        {
            var grid = new FakeGrid(5, 5);

            var path = AStarSearch.FindPath(grid, (0, 0), (3, 0), out int pathLength);

            Assert.IsNotNull(path);
            Assert.AreEqual(4, path.Count);
            Assert.AreEqual((0, 0), path[0]);
            Assert.AreEqual((3, 0), path[path.Count - 1]);
        }

        [Test]
        public void FindPath_AroundObstacle_AvoidsBlockedCells()
        {
            var blocked = new List<(int, int)> { (1, 0), (1, 1), (1, 2) };
            var grid = new FakeGrid(5, 5, blocked);

            var path = AStarSearch.FindPath(grid, (0, 0), (2, 0), out _);

            Assert.IsNotNull(path);
            Assert.IsFalse(path.Any(p => blocked.Contains(p)));
        }

        [Test]
        public void FindPath_NoPathAvailable_ReturnsNullAndZeroLength()
        {
            var blocked = new List<(int, int)> { (1, 0), (1, 1), (1, 2), (1, 3), (1, 4) };
            var grid = new FakeGrid(5, 5, blocked);

            var path = AStarSearch.FindPath(grid, (0, 0), (4, 0), out int pathLength);

            Assert.IsNull(path);
            Assert.AreEqual(0, pathLength);
        }

        [Test]
        public void FindPath_SameStartAndEnd_ReturnsSinglePositionPath()
        {
            var grid = new FakeGrid(3, 3);

            var path = AStarSearch.FindPath(grid, (1, 1), (1, 1), out int pathLength);

            Assert.IsNotNull(path);
            Assert.AreEqual(1, path.Count);
            Assert.AreEqual((1, 1), path[0]);
            Assert.AreEqual(0, pathLength);
        }
    }
}
