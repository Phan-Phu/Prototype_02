using System;

namespace Domain
{
    public interface IInteractable
    {
        void Interact(Action onInteractComplete);
    }
}
