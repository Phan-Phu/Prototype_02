using Domain;
using UnityEngine;

namespace Application
{
    public class GrenadeExplodedEvent : GameEvent
    {
        public Vector3 Position { get; }

        public GrenadeExplodedEvent(Vector3 position)
        {
            Position = position;
        }
    }

    public class CrateDestroyedEvent : GameEvent
    {
        public GridPosition GridPosition { get; }

        public CrateDestroyedEvent(GridPosition gridPosition)
        {
            GridPosition = gridPosition;
        }
    }

    public class DoorStateChangedEvent : GameEvent
    {
        public Door Door { get; }
        public bool IsOpen { get; }

        public DoorStateChangedEvent(Door door, bool isOpen)
        {
            Door = door;
            IsOpen = isOpen;
        }
    }
}
