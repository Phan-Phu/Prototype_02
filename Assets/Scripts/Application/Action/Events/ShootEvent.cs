using Domain;

namespace Application
{
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
}
