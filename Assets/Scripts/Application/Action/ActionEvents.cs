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

    public class ActionCompletedEvent : GameEvent
    {
        public BaseAction Action { get; }

        public ActionCompletedEvent(BaseAction action)
        {
            Action = action;
        }
    }

    public class MoveStartedEvent : GameEvent
    {
        public MoveAction MoveAction { get; }

        public MoveStartedEvent(MoveAction moveAction)
        {
            MoveAction = moveAction;
        }
    }

    public class MoveStoppedEvent : GameEvent
    {
        public MoveAction MoveAction { get; }

        public MoveStoppedEvent(MoveAction moveAction)
        {
            MoveAction = moveAction;
        }
    }

    public class ShootEvent : GameEvent
    {
        public ShootAction ShootAction { get; }
        public Unit ShootingUnit { get; }
        public Unit TargetUnit { get; }

        public ShootEvent(ShootAction shootAction, Unit shootingUnit, Unit targetUnit)
        {
            ShootAction = shootAction;
            ShootingUnit = shootingUnit;
            TargetUnit = targetUnit;
        }
    }

    public class SwordActionStartedEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordActionStartedEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }

    public class SwordHitEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordHitEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }

    public class SwordActionCompletedEvent : GameEvent
    {
        public SwordAction SwordAction { get; }

        public SwordActionCompletedEvent(SwordAction swordAction)
        {
            SwordAction = swordAction;
        }
    }
}
