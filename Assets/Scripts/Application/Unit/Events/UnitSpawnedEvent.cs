using Domain;

namespace Application
{
    public class UnitSpawnedEvent : GameEvent
    {
        public Unit Unit { get; }

        public UnitSpawnedEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
