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

    public class SelectedActionChangedEvent : GameEvent
    {
        public BaseAction Action { get; }

        public SelectedActionChangedEvent(BaseAction action)
        {
            Action = action;
        }
    }

    public class BusyChangedEvent : GameEvent
    {
        public bool IsBusy { get; }

        public BusyChangedEvent(bool isBusy)
        {
            IsBusy = isBusy;
        }
    }
}
