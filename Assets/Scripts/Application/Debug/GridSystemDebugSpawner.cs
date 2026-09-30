using UnityEngine;
using Domain;
using Infrastructure;

namespace Application
{
    // Debug-only tooling: instantiates one debug prefab per grid cell and feeds it the cell's
    // payload through IGridDebugVisual, never the concrete component (e.g. GridDebugObject).
    public class GridSystemDebugSpawner<TGridObject>
    {
        public void Spawn(IGridSystemHexWorld<TGridObject> gridSystem, Transform debugObjectPrefab)
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
