using UnityEngine;
using DodoRun.Event;
using DodoRun.PowerUps;

namespace DodoRun.UI.Controllers
{
    public sealed class PowerupUIController
    {
        private readonly PowerupBar magnet;
        private readonly PowerupBar shield;
        private readonly PowerupBar doubleScore;
        private readonly EventService events;

        public PowerupUIController(
            DodoRun.UI.PowerupUIRefs refs,
            EventService events)
        {
            this.events = events;

            magnet = new PowerupBar(refs.magnet);
            shield = new PowerupBar(refs.shield);
            doubleScore = new PowerupBar(refs.doubleScore);

            events.OnPowerupActivated.AddListner(OnActivated);
            events.OnPowerupExpired.AddListner(OnExpired);
        }

        private void OnActivated(PowerupType type, float duration)
        {
            Get(type)?.Activate(duration);
        }

        private void OnExpired(PowerupType type)
        {
            Get(type)?.Deactivate();
        }

        private PowerupBar Get(PowerupType type)
        {
            return type switch
            {
                PowerupType.Magnet => magnet,
                PowerupType.Shield => shield,
                PowerupType.DoubleScore => doubleScore,
                _ => null
            };
        }

        public void Dispose()
        {
            events.OnPowerupActivated.RemoveListner(OnActivated);
            events.OnPowerupExpired.RemoveListner(OnExpired);
        }

        public void Update(float value)
        {
            magnet.Update(value);
            shield.Update(value);
            doubleScore.Update(value);
        }

        private sealed class PowerupBar
        {
            private readonly UnityEngine.UI.Slider slider;
            private float drainSpeed;
            private bool active;

            public PowerupBar(UnityEngine.UI.Slider slider)
            {
                this.slider = slider;
                slider.gameObject.SetActive(false);
            }

            public void Activate(float duration)
            {
                slider.gameObject.SetActive(true);
                slider.value = 1f;
                drainSpeed = 1f / duration;
                active = true;
            }

            public void Deactivate()
            {
                active = false;
                slider.value = 0f;
                slider.gameObject.SetActive(false);
            }

            public void Update(float delta)
            {
                if (!active) return;
                slider.value -= drainSpeed * delta;
                if (slider.value <= 0f) Deactivate();
            }
        }
    }
}
