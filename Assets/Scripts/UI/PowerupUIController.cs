using UnityEngine;
using UnityEngine.UI;
using DodoRun.Main;
using DodoRun.PowerUps;

namespace DodoRun.UI
{
    public sealed class PowerupUIController : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Slider magnetBar;
        [SerializeField] private Slider shieldBar;
        [SerializeField] private Slider doubleScoreBar;

        private PowerupBar magnet;
        private PowerupBar shield;
        private PowerupBar doubleScore;

        private void Start()
        {
            magnet = new PowerupBar(magnetBar);
            shield = new PowerupBar(shieldBar);
            doubleScore = new PowerupBar(doubleScoreBar);

            var events = GameService.Instance.EventService;
            events.OnPowerupActivated.AddListner(OnActivated);
            events.OnPowerupExpired.AddListner(OnExpired);

            ResetAll();
        }

        private void OnDestroy()
        {
            if (GameService.Instance == null) return;

            var events = GameService.Instance.EventService;
            events.OnPowerupActivated.RemoveListner(OnActivated);
            events.OnPowerupExpired.RemoveListner(OnExpired);
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;

            magnet.Update(delta);
            shield.Update(delta);
            doubleScore.Update(delta);
        }

        private void OnActivated(PowerupType type, float duration)
        {
            GetBar(type)?.Activate(duration);
        }

        private void OnExpired(PowerupType type)
        {
            GetBar(type)?.Deactivate();
        }

        private PowerupBar GetBar(PowerupType type)
        {
            return type switch
            {
                PowerupType.Magnet => magnet,
                PowerupType.Shield => shield,
                PowerupType.DoubleScore => doubleScore,
                _ => null
            };
        }

        private void ResetAll()
        {
            magnet.Reset();
            shield.Reset();
            doubleScore.Reset();
        }

        private sealed class PowerupBar
        {
            private readonly Slider slider;
            private float drainSpeed;
            private bool active;

            public PowerupBar(Slider slider)
            {
                this.slider = slider;
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

                if (slider.value <= 0f)
                    Deactivate();
            }

            public void Reset()
            {
                active = false;
                slider.value = 0f;
                slider.gameObject.SetActive(false);
            }
        }
    }
}
