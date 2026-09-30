using Domain;

namespace Application
{
    public class SwordHitEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordHitEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }
}
