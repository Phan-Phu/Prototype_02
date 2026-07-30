using UnityEngine;

namespace Domain
{
    // Contract only - the hex coordinate math and storage live in Infrastructure/Grid/GridSystemHex.cs.
    public interface IGridSystemHex<TGridObject>
    {
        Vector3 GetWorldPosition(GridPosition gridPosition);
        GridPosition GetGridPosition(Vector3 worldPosition);
        TGridObject GetGridObject(GridPosition gridPosition);
        bool IsValidGridPosition(GridPosition gridPosition);
        int GetWidth();
        int GetHeight();
    }
}
