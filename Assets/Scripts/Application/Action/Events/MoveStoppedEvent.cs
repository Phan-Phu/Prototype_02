using Domain;

namespace Application
{
    public class MoveStoppedEvent : GameEvent
    {
        public MoveAction MoveAction { get; }

        public MoveStoppedEvent(MoveAction moveAction)
        {
            MoveAction = moveAction;
        }
    }
}
