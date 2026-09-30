using Domain;

namespace Application
{
    public class SelectedActionChangedEvent : GameEvent
    {
        public BaseAction Action { get; }

        public SelectedActionChangedEvent(BaseAction action)
        {
            Action = action;
        }
    }
}
