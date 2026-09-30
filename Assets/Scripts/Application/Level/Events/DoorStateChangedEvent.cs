using Domain;

namespace Application
{
    public class DoorStateChangedEvent : GameEvent
    {
        public Door Door { get; }
        public bool IsOpen { get; }

        public DoorStateChangedEvent(Door door, bool isOpen)
        {
            Door = door;
            IsOpen = isOpen;
        }
    }
}
