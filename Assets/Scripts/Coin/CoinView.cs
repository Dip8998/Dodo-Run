using DodoRun.Main;
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
            other.gameObject.TryGetComponent(out PlayerView player);
            if (player)
            {
                coinController.CollectCoin();
            }
        }
    }
}
