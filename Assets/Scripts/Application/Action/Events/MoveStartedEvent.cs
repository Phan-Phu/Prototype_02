using Domain;

namespace Application
{
    public class MoveStartedEvent : GameEvent
    {
        public MoveAction MoveAction { get; }

        public MoveStartedEvent(MoveAction moveAction)
        {
            MoveAction = moveAction;
        }
    }
}
