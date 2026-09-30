using Domain;

namespace Application
{
    public class BusyChangedEvent : GameEvent
    {
        public bool IsBusy { get; }

        public BusyChangedEvent(bool isBusy)
        {
            IsBusy = isBusy;
        }
    }
}
