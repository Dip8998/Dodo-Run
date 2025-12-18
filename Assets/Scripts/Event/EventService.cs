using DodoRun.PowerUps;
using UnityEngine;

namespace DodoRun.Event
{
    public class EventService
    {
        public EventController<Transform> OnPlayerSpawned {  get; private set; }
        public EventController<PowerupType, float> OnPowerupActivated;
        public EventController<PowerupType> OnPowerupExpired;
        public EventController<int> OnCoinCollected;

        public EventService()
        {
            OnPlayerSpawned = new EventController<Transform>();
            OnPowerupActivated = new EventController<PowerupType, float>();
            OnPowerupExpired = new EventController<PowerupType>();
            OnCoinCollected = new EventController<int>();
        }
    }
}
