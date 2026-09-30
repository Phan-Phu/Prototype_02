using Domain;
using UnityEngine;

namespace Application
{
    public class GrenadeExplodedEvent : GameEvent
    {
        public Vector3 Position { get; }

        public GrenadeExplodedEvent(Vector3 position)
        {
            Position = position;
        }
    }
}
