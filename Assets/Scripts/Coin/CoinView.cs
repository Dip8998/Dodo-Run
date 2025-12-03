using DodoRun.Player;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinView : MonoBehaviour
    {
        private CoinController coinController;

        public void SetController(CoinController controller)
        {
            coinController = controller;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (coinController == null) return;

            if (other.TryGetComponent<PlayerView>(out _))
            {
                coinController.CollectCoin();
            }
        }
    }
}
