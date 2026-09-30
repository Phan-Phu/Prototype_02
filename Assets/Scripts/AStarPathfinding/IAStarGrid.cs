using System;
using System.Collections.Generic;

namespace AStarPathfinding
{
    // Grid-agnostic contract AStarSearch depends on. Implementations describe topology
    // (neighbours), traversability and costs for a specific grid; the algorithm itself has
    // no knowledge of hex/square grids, Unity types, or game-specific state.
    public interface IAStarGrid<TPosition> where TPosition : IEquatable<TPosition>
    {
        bool IsWalkable(TPosition position);

        IEnumerable<TPosition> GetNeighbours(TPosition position);

        int GetMoveCost(TPosition from, TPosition to);

        int GetHeuristicCost(TPosition from, TPosition to);
    }
}
