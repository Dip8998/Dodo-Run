using System;

namespace DodoRun.Event
{
    public class EventController<T>
    {
        public event Action<T> baseEvent;
        public void InvokeEvent(T type) => baseEvent?.Invoke(type);
        public void AddListner(Action<T> type) => baseEvent += type;
        public void RemoveListner(Action<T> type) => baseEvent -= type;
    }
}