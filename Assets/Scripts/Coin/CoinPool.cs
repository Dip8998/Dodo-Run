using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinPool
    {
        private readonly List<PooledCoin> coins = new List<PooledCoin>();
        private readonly Stack<PooledCoin> freeCoins = new Stack<PooledCoin>();
        private readonly CoinView coinPrefab;

        public CoinPool(CoinView coinPrefab, int initialCount = 10)
        {
            this.coinPrefab = coinPrefab;
        }

        public CoinController GetCoin(Vector3 spawnPos)
        {
            if (freeCoins.Count > 0)
            {
                PooledCoin pooled = freeCoins.Pop();
                pooled.isUsed = true;
                pooled.Controller.ResetCoin(coinPrefab, spawnPos);
                return pooled.Controller;
            }

            return CreateNewCoin(spawnPos);
        }

        public void ReturnCoinToPool(CoinController returnedCoin)
        {
            if (returnedCoin == null) return;

            for (int i = 0; i < coins.Count; i++)
            {
                PooledCoin pooled = coins[i];
                if (pooled.Controller == returnedCoin && pooled.isUsed)
                {
                    pooled.isUsed = false;
                    freeCoins.Push(pooled);
                    break;
                }
            }
        }

        private CoinController CreateNewCoin(Vector3 spawnPos)
        {
            PooledCoin pooled = new PooledCoin
            {
                Controller = new CoinController(coinPrefab, spawnPos),
                isUsed = true
            };

            coins.Add(pooled);
            return pooled.Controller;
        }

        public class PooledCoin
        {
            public CoinController Controller;
            public bool isUsed;
        }
    }
}
