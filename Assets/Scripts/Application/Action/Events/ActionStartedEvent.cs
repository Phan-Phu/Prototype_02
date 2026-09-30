using Domain;

namespace Application
{
    public class ActionStartedEvent : GameEvent
    {
        public BaseAction Action { get; }

        public ActionStartedEvent(BaseAction action)
        {
            Action = action;
        }
    }
}
