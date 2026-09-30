using Domain;

namespace Application
{
    public class SelectedUnitChangedEvent : GameEvent
    {
        public Unit Unit { get; }

        public SelectedUnitChangedEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
