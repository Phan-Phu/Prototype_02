using Domain;

namespace Application
{
    public class ActionCompletedEvent : GameEvent
    {
        public BaseAction Action { get; }

        public ActionCompletedEvent(BaseAction action)
        {
            Action = action;
        }
    }
}
