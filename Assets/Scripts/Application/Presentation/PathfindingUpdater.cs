using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class PathfindingUpdater : MonoBehaviour
    {
        private void Start()
        {
            DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
        }

        private void DestructibleCrate_OnAnyDestroyed(object sender, EventArgs e)
        {
            DestructibleCrate destructibleCrate = sender as DestructibleCrate;
            Pathfinding.Instance.SetIsWalkableGridPositon(destructibleCrate.GetGridPosition(), true);
        }
    }

}