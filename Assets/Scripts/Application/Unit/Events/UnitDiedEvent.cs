using Domain;

namespace Application
{
    public class UnitDiedEvent : GameEvent
    {
        public Unit Unit { get; }

        public UnitDiedEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
