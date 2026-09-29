using Domain;

namespace Application
{
    public class UnitGridPositionChangedEvent : GameEvent
    {
        public Unit Unit { get; }
        public GridPosition From { get; }
        public GridPosition To { get; }

        public UnitGridPositionChangedEvent(Unit unit, GridPosition from, GridPosition to)
        {
            Unit = unit;
            From = from;
            To = to;
        }
    }
}
