using System.Collections.Generic;

namespace Domain
{
    // Contract only - the A* search itself lives in Infrastructure/Pathfinding/AStarPathfinder.cs.
    public interface IPathfindingAlgorithm
    {
        List<GridPosition> FindPath(IGridSystemHex<PathNode> gridSystem, GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength);
    }
}
