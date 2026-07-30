using UnityEngine;
using Domain;

namespace Infrastructure
{
    // Debug-only tooling: instantiates one debug prefab per grid cell and feeds it the cell's
    // payload. Depends only on the Domain-defined IGridDebugVisual interface, never the concrete
    // Application-layer component that implements it (e.g. GridDebugObject).
    public class GridSystemDebugSpawner<TGridObject>
    {
        public void Spawn(IGridSystemHex<TGridObject> gridSystem, Transform debugObjectPrefab)
        {
            for (int i = 0; i < gridSystem.GetWidth(); i++)
            {
                for (int j = 0; j < gridSystem.GetHeight(); j++)
                {
                    GridPosition gridPosition = new GridPosition(i, j);
                    Transform debugTransform = GameObject.Instantiate(debugObjectPrefab, gridSystem.GetWorldPosition(gridPosition), Quaternion.identity);
                    IGridDebugVisual gridDebugVisual = debugTransform.GetComponent<IGridDebugVisual>();
                    gridDebugVisual?.SetGridObject(gridSystem.GetGridObject(gridPosition));
                }
            }
        }
    }
}
