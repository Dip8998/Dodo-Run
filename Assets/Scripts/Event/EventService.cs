using UnityEngine;
using DodoRun.PowerUps;

namespace DodoRun.Event
{
    public sealed class EventService
    {
        public EventController<Transform> OnPlayerSpawned { get; } = new();
        public EventController<int> OnCoinCollected { get; } = new();
        public EventController<PowerupType, float> OnPowerupActivated { get; } = new();
        public EventController<PowerupType> OnPowerupExpired { get; } = new();
    }
}
