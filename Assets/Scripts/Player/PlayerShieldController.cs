using UnityEngine;
using DodoRun.Main;
using DodoRun.PowerUps;

namespace DodoRun.Player
{
    public sealed class PlayerShieldController : MonoBehaviour
    {
        [SerializeField] private GameObject shieldObject;

        private void Start()
        {
            shieldObject.SetActive(false);

            GameService.Instance.EventService.OnPowerupActivated.AddListner(OnActivated);
            GameService.Instance.EventService.OnPowerupExpired.AddListner(OnExpired);
        }

        private void OnDestroy()
        {
            if (GameService.Instance == null) return;

            GameService.Instance.EventService.OnPowerupActivated.RemoveListner(OnActivated);
            GameService.Instance.EventService.OnPowerupExpired.RemoveListner(OnExpired);
        }

        private void OnActivated(PowerupType type, float _)
        {
            if (type == PowerupType.Shield)
                shieldObject.SetActive(true);
        }

        private void OnExpired(PowerupType type)
        {
            if (type == PowerupType.Shield)
                shieldObject.SetActive(false);
        }
    }
}
