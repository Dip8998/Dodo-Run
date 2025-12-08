using UnityEngine;
using DodoRun.Main;

namespace DodoRun.PowerUps
{
    public class PowerupController
    {
        public PowerupView View { get; private set; }
        public PowerupType Type { get; private set; }

        private bool isUsed;

        public PowerupController(PowerupView prefab, PowerupType type, Vector3 position)
        {
            Reset(prefab, type, position);
        }

        public void Reset(PowerupView prefab, PowerupType type, Vector3 position)
        {
            if (View == null)
            {
                View = Object.Instantiate(prefab, position, Quaternion.identity);
                View.SetController(this);
            }
            else
            {
                View.transform.position = position;
                View.gameObject.SetActive(true);
            }

            Type = type;
            isUsed = true;
        }

        public void CollectPowerup()
        {
            GameService.Instance.PowerupService.ActivatePowerup(Type);
            Deactivate();
        }

        public void Deactivate()
        {
            if (View != null)
                View.gameObject.SetActive(false);

            isUsed = false;
            GameService.Instance.PowerupService.ReturnToPool(this);
        }

        public bool IsUsed() => isUsed;
    }
}
