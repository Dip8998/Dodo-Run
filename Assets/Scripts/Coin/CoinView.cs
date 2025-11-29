using UnityEngine;
using DodoRun.Player;
using DodoRun.Main;

namespace DodoRun.Coin
{
    public class CoinView : MonoBehaviour
    {
        private CoinController coinController;
        private Transform player;
        private float despawnDistance = 5f;

        public void SetController(CoinController controller)
        {
            coinController = controller;
            player = GameService.Instance.PlayerService.GetPlayerTransform();
        }

        private void Update()
        {
            if (!GameService.Instance.IsGameRunning) return;
            if (player == null) return;

            if (transform.position.z < player.position.z - despawnDistance)
                coinController.Deactivate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerView>(out _))
                coinController.CollectCoin();
        }
    }
}
