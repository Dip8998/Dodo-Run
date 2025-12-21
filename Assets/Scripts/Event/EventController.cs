using System;

namespace DodoRun.Event
{
    public sealed class EventController<T>
    {
        private Action<T> listeners;

        public void AddListner(Action<T> action)
        {
            listeners += action;
        }

        public void RemoveListner(Action<T> action)
        {
            listeners -= action;
        }

        public void InvokeEvent(T param)
        {
            listeners?.Invoke(param);
        }

        public void Clear()
        {
            listeners = null;
        }
    }

    public sealed class EventController<T1, T2>
    {
        private Action<T1, T2> listeners;

        public void AddListner(Action<T1, T2> action)
        {
            listeners += action;
        }

        public void RemoveListner(Action<T1, T2> action)
        {
            listeners -= action;
        }

        public void InvokeEvent(T1 a, T2 b)
        {
            listeners?.Invoke(a, b);
        }

        public void Clear()
        {
            listeners = null;
        }
    }
}
