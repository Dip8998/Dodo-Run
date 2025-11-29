using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinService
    {
        public CoinView CoinPrefab { get; private set; }
        public float BaseVerticalOffset { get; private set; }

        private readonly CoinPool coinPool;
        private readonly List<CoinController> activeCoins = new List<CoinController>();

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
    }
}