using Domain;

namespace Application
{
    public class UnitSpawnedEvent : GameEvent
    {
        public Unit Unit { get; }

        public UnitSpawnedEvent(Unit unit)
        {
            Unit = unit;
        }
    }

    public class UnitDiedEvent : GameEvent
    {
        public Unit Unit { get; }

        public UnitDiedEvent(Unit unit)
        {
            Unit = unit;
        }
    }

    public class HealthDamagedEvent : GameEvent
    {
        public HealthSystem HealthSystem { get; }

        public HealthDamagedEvent(HealthSystem healthSystem)
        {
            HealthSystem = healthSystem;
        }
    }

    public class HealthDepletedEvent : GameEvent
    {
        public HealthSystem HealthSystem { get; }

        public HealthDepletedEvent(HealthSystem healthSystem)
        {
            HealthSystem = healthSystem;
        }
    }
}
