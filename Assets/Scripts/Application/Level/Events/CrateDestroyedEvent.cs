using Domain;

namespace Application
{
    public class CrateDestroyedEvent : GameEvent
    {
        public GridPosition GridPosition { get; }

        public CrateDestroyedEvent(GridPosition gridPosition)
        {
            GridPosition = gridPosition;
        }
    }
}
