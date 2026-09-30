using Domain;

namespace Application
{
    public class SwordActionCompletedEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordActionCompletedEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }
}
