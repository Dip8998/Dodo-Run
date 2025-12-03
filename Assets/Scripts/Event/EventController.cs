using System;
using System.Collections.Generic;

namespace DodoRun.Event
{
    public class EventController<T>
    {
        private readonly List<Action<T>> listeners = new List<Action<T>>();

        public void InvokeEvent(T param)
        {
            for (int i = 0; i < listeners.Count; i++)
                listeners[i]?.Invoke(param);
        }

        public void AddListner(Action<T> action)
        {
            if (!listeners.Contains(action))
                listeners.Add(action);
        }

        public void RemoveListner(Action<T> action)
        {
            listeners.Remove(action);
        }
    }
}