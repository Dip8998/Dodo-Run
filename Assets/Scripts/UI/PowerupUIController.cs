using UnityEngine;
using UnityEngine.UI;
using DodoRun.Main;
using DodoRun.PowerUps;
using System.Collections;

namespace DodoRun.UI
{
    public class PowerupUIController : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Slider magnetBar;
        [SerializeField] private Slider shieldBar;
        [SerializeField] private Slider doubleScoreBar;

        private bool magnetActive;
        private bool shieldActive;
        private bool doubleScoreActive;

        private PowerupBarData magnetData;
        private PowerupBarData shieldData;
        private PowerupBarData doubleScoreData;


        private void Start()
        {
            magnetData = magnetBar.GetComponent<PowerupBarData>();
            shieldData = shieldBar.GetComponent<PowerupBarData>();
            doubleScoreData = doubleScoreBar.GetComponent<PowerupBarData>();

            GameService.Instance.EventService.OnPowerupActivated.AddListner(HandlePowerupActivated);
            GameService.Instance.EventService.OnPowerupExpired.AddListner(HandlePowerupExpired);

            ResetAllBars();
        }


        private void OnDestroy()
        {
            if (GameService.Instance == null) return;

            var powerupService = GameService.Instance.PowerupService;
            if (powerupService == null) return;

            GameService.Instance.EventService.OnPowerupActivated.RemoveListner(HandlePowerupActivated);
            GameService.Instance.EventService.OnPowerupExpired.RemoveListner(HandlePowerupExpired);
        }

        private void HandlePowerupActivated(PowerupType type, float duration)
        {
            switch (type)
            {
                case PowerupType.Magnet:
                    ActivateBar(magnetBar, magnetData, ref magnetActive, duration);
                    break;

                case PowerupType.Shield:
                    ActivateBar(shieldBar, shieldData, ref shieldActive, duration);
                    break;

                case PowerupType.DoubleScore:
                    ActivateBar(doubleScoreBar, doubleScoreData, ref doubleScoreActive, duration);
                    break;
            }
        }

        private void HandlePowerupExpired(PowerupType type)
        {
            switch (type)
            {
                case PowerupType.Magnet:
                    DeactivateBar(magnetBar, ref magnetActive);
                    break;

                case PowerupType.Shield:
                    DeactivateBar(shieldBar, ref shieldActive);
                    break;

                case PowerupType.DoubleScore:
                    DeactivateBar(doubleScoreBar, ref doubleScoreActive);
                    break;
            }
        }

        private void ActivateBar(Slider slider, PowerupBarData data, ref bool active, float duration)
        {
            slider.gameObject.SetActive(true);
            slider.value = 1f;

            data.DrainSpeed = 1f / duration;
            active = true;
        }

        private void DeactivateBar(Slider slider, ref bool active)
        {
            active = false;
            slider.value = 0f;
            slider.gameObject.SetActive(false);
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;

            Drain(magnetBar, magnetData, ref magnetActive, delta);
            Drain(shieldBar, shieldData, ref shieldActive, delta);
            Drain(doubleScoreBar, doubleScoreData, ref doubleScoreActive, delta);
        }

        private void Drain(Slider slider, PowerupBarData data, ref bool active, float delta)
        {
            if (!active || !slider.gameObject.activeSelf)
                return;

            slider.value -= data.DrainSpeed * delta;

            if (slider.value <= 0f)
            {
                slider.value = 0f;
                active = false;
                slider.gameObject.SetActive(false);
            }
        }

        private void ResetAllBars()
        {
            magnetBar.value = 0f;
            shieldBar.value = 0f;
            doubleScoreBar.value = 0f;

            magnetBar.gameObject.SetActive(false);
            shieldBar.gameObject.SetActive(false);
            doubleScoreBar.gameObject.SetActive(false);
        }
    }
}
