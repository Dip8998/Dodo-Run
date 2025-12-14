using UnityEngine;
using DodoRun.Main;
using DodoRun.PowerUps;

namespace DodoRun.Player
{
    public class PlayerShieldController : MonoBehaviour
    {
        [SerializeField] private GameObject shieldObject;

        private void Start()
        {
            if (shieldObject != null)
                shieldObject.SetActive(false);

            GameService.Instance.EventService.OnPowerupActivated
                .AddListner(OnPowerupActivated);

            GameService.Instance.EventService.OnPowerupExpired
                .AddListner(OnPowerupExpired);
        }

        private void OnDestroy()
        {
            if (GameService.Instance == null)
                return;

            GameService.Instance.EventService.OnPowerupActivated
                .RemoveListner(OnPowerupActivated);

            GameService.Instance.EventService.OnPowerupExpired
                .RemoveListner(OnPowerupExpired);
        }

        private void OnPowerupActivated(PowerupType type, float duration)
        {
            if (type != PowerupType.Shield)
                return;

            if (shieldObject != null)
                shieldObject.SetActive(true);
        }

        private void OnPowerupExpired(PowerupType type)
        {
            if (type != PowerupType.Shield)
                return;

            if (shieldObject != null)
                shieldObject.SetActive(false);
        }
    }
}
