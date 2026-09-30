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
        int GetWidth();
        int GetHeight();
    }
}
