using Domain;
using System;
using System.Collections.Generic;

namespace Application
{
    // Static, type-keyed event bus - the single channel for game events. Listeners filter by the
    // event's payload when they only care about one object (e.g. their own Unit). Every
    // AddListener must be paired with a RemoveListener (OnDisable/OnDestroy), since this class
    // is static and outlives scenes.
    public static class EventManager
    {
        private static readonly Dictionary<Type, Action<GameEvent>> listeners = new Dictionary<Type, Action<GameEvent>>();
        private static readonly Dictionary<Delegate, Action<GameEvent>> wrappers = new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> listener) where T : GameEvent
        {
            void Wrapper(GameEvent gameEvent) => listener((T)gameEvent);
            wrappers[listener] = Wrapper;

            if (listeners.TryGetValue(typeof(T), out Action<GameEvent> existing))
            {
                listeners[typeof(T)] = existing + Wrapper;
            }
            else
            {
                listeners[typeof(T)] = Wrapper;
            }
        }

        public static void RemoveListener<T>(Action<T> listener) where T : GameEvent
        {
            if (!wrappers.TryGetValue(listener, out Action<GameEvent> wrapper))
            {
                return;
            }

            wrappers.Remove(listener);

            if (!listeners.TryGetValue(typeof(T), out Action<GameEvent> existing))
            {
                return;
            }

            existing -= wrapper;
            if (existing == null)
            {
                listeners.Remove(typeof(T));
            }
            else
            {
                listeners[typeof(T)] = existing;
            }
        }

        public static void Broadcast(GameEvent gameEvent)
        {
            if (listeners.TryGetValue(gameEvent.GetType(), out Action<GameEvent> handlers))
            {
                handlers?.Invoke(gameEvent);
            }
        }
    }
}
