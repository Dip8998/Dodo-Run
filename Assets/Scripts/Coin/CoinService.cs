using System.Collections.Generic;
using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinService
    {
        public CoinView CoinPrefab { get; private set; }
        public float BaseVerticalOffset { get; private set; }

        private readonly CoinPool coinPool;
        private readonly List<CoinController> activeCoins = new List<CoinController>();

        private Transform playerTransform;
        private readonly float despawnDistance = 5f;

        public IReadOnlyList<CoinController> ActiveCoins => activeCoins;

        public CoinService(CoinView coinPrefab, float verticalOffset)
        {
            CoinPrefab = coinPrefab;
            BaseVerticalOffset = verticalOffset;
            coinPool = new CoinPool(CoinPrefab);
        }

        public CoinController GetCoin(Vector3 spawnPos)
        {
            CoinController coinController = coinPool.GetCoin(spawnPos);
            activeCoins.Add(coinController);
            return coinController;
        }

        public void ReturnCoinToPool(CoinController coinController)
        {
            if (coinController == null) return;

            if (activeCoins.Contains(coinController))
            {
                activeCoins.Remove(coinController);
            }

            coinPool.ReturnCoinToPool(coinController);
        }

        public void ClearAllActiveCoins()
        {
            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                activeCoins[i].Deactivate();
            }

            activeCoins.Clear();
        }

        public void UpdateCoins()
        {
            if (!GameService.Instance.IsGameRunning) return;

            if (playerTransform == null)
            {
                playerTransform = GameService.Instance.PlayerService.GetPlayerTransform();
                if (playerTransform == null)
                    return;
            }

            float playerZ = playerTransform.position.z;

            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                CoinController coin = activeCoins[i];
                CoinView view = coin.CoinView;

                if (view == null || !view.gameObject.activeInHierarchy)
                    continue;

                if (view.transform.position.z < playerZ - despawnDistance)
                {
                    coin.Deactivate();
                }
            }
        }

        public void ReleaseAllMagnetCoins()
        {
            foreach (var c in ActiveCoins)
                c.IsBeingPulled = false;
        }

    }
}
