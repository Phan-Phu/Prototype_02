using System.Collections.Generic;

namespace Domain
{
    // Engine-free hex grid contract: storage, bounds and hex topology only. Grid <-> world
    // conversion needs UnityEngine types, so it lives in Infrastructure (IGridSystemHexWorld<T>).
    public interface IGridSystemHex<TGridObject>
    {
        TGridObject GetGridObject(GridPosition gridPosition);
        bool IsValidGridPosition(GridPosition gridPosition);
        int GetDistance(GridPosition from, GridPosition to);
        List<GridPosition> GetNeighbours(GridPosition gridPosition);
        // Every valid cell whose hex distance from center is at most range (center included).
        List<GridPosition> GetGridPositionsInRange(GridPosition center, int range);
        int GetWidth();
        int GetHeight();
    }
}
