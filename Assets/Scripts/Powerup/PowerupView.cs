using UnityEngine;
using DodoRun.Player;

namespace DodoRun.PowerUps
{
    public sealed class PowerupView : MonoBehaviour
    {
        private PowerupController controller;

        public void SetController(PowerupController controller)
        {
            this.controller = controller;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (controller == null) return;

            if (other.TryGetComponent<PlayerView>(out _))
                controller.Collect();
        }
    }
}
