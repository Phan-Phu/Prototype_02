using Domain;

namespace Application
{
    public class UnitActionPointsChangedEvent : GameEvent
    {
        public Unit Unit { get; }
        public int ActionPoints { get; }

        public UnitActionPointsChangedEvent(Unit unit, int actionPoints)
        {
            Unit = unit;
            ActionPoints = actionPoints;
        }
    }
}
