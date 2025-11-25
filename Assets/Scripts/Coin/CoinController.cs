using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinController
    {
        public CoinView CoinView { get; private set; }
        private bool isUsed = true;

        public CoinController(CoinView coinView, Vector3 spawnPos)
        {
            SetupView(coinView, spawnPos);
        }

        private void SetupView(CoinView coinView, Vector3 spawnPos)
        {
            CoinView = Object.Instantiate(coinView, spawnPos, Quaternion.identity);

            CoinView.SetController(this);
            isUsed = true;
        }

        public void CollectCoin()
        {
            Deactivate();
        }

        public void ResetCoin(CoinView coinView, Vector3 spawnPos)
        {
            CoinView.transform.position = spawnPos;
            CoinView.gameObject.SetActive(true);
            isUsed = true;
        }

        public void Deactivate()
        {
            CoinView.gameObject?.SetActive(false);
            isUsed = false;
            GameService.Instance.CoinService.ReturnCoinToPool(this);
        }

        public bool IsUsed() => isUsed;
    }
}