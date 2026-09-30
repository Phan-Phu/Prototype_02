using Domain;

namespace Application
{
    public class SwordActionStartedEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordActionStartedEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }
}
