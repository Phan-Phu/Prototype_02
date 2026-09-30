using Domain;

namespace Application
{
    public class HealthDamagedEvent : GameEvent
    {
        public HealthSystem HealthSystem { get; }

        public HealthDamagedEvent(HealthSystem healthSystem)
        {
            HealthSystem = healthSystem;
        }
    }
}
