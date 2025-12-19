using UnityEngine;
using DodoRun.Player;

namespace DodoRun.Coin
{
    public sealed class CoinView : MonoBehaviour
    {
        private CoinController controller;

        public void SetController(CoinController controller)
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
