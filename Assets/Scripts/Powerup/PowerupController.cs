using UnityEngine;
using DodoRun.Main;

namespace DodoRun.PowerUps
{
    public sealed class PowerupController
    {
        public PowerupView View { get; private set; }
        public PowerupType Type { get; private set; }

        private bool isUsed;

        public PowerupController(PowerupView prefab, PowerupType type, Vector3 pos)
        {
            Initialize(prefab, type, pos);
        }

        public void Initialize(PowerupView prefab, PowerupType type, Vector3 pos)
        {
            if (View == null)
            {
                View = Object.Instantiate(prefab, pos, Quaternion.identity);
                View.SetController(this);
            }
            else
            {
                View.transform.position = pos;
                View.gameObject.SetActive(true);
            }

            Type = type;
            isUsed = true;
        }

        public void Collect()
        {
            GameService.Instance.PowerupService.ActivatePowerup(Type);
            Deactivate();
        }

        public void Deactivate()
        {
            if (!isUsed) return;

            isUsed = false;
            View.gameObject.SetActive(false);
            GameService.Instance.PowerupService.ReturnToPool(this);
        }

        public bool IsUsed() => isUsed;
    }
}
