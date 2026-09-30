using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace Application
{
    // Debug label for one pathfinding cell: green when walkable, red when blocked.
    // Expects its grid object to be the cell's GridPosition.
    public class PathfindingGridDebugObject : GridDebugObject
    {
        [SerializeField] private SpriteRenderer spriteRendererIsWalkable;
        private GridPosition gridPosition;

        public override void SetGridObject(object gridObject)
        {
            base.SetGridObject(gridObject);
            gridPosition = (GridPosition)gridObject;
        }

        protected override void Update()
        {
            base.Update();
            spriteRendererIsWalkable.color = Pathfinding.Instance.IsWalkableGridPositon(gridPosition) ? Color.green : Color.red;
        }
    }

}
