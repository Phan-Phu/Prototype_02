using Domain;

namespace Application
{
    public class HealthDepletedEvent : GameEvent
    {
        public HealthSystem HealthSystem { get; }

        public HealthDepletedEvent(HealthSystem healthSystem)
        {
            HealthSystem = healthSystem;
        }
    }
}
