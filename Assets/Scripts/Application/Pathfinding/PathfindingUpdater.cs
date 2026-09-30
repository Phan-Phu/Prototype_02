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
            EventManager.AddListener<CrateDestroyedEvent>(OnCrateDestroyedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<CrateDestroyedEvent>(OnCrateDestroyedEvent);
        }

        private void OnCrateDestroyedEvent(CrateDestroyedEvent @event)
        {
            Pathfinding.Instance.SetIsWalkableGridPositon(@event.GridPosition, true);
        }
    }

}