using UnityEngine;

namespace DodoRun.Event
{
    public class EventService
    {
        public EventController<Transform> OnPlayerSpawned {  get; private set; }

        public EventService()
        {
            OnPlayerSpawned = new EventController<Transform>();
        }
    }
}
