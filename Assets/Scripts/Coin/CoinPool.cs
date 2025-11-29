using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinPool
    {
        private readonly List<PooledCoin> coins = new List<PooledCoin>();
        private readonly CoinView coinPrefab;

        public CoinPool(CoinView coinPrefab, int initialCount = 10)
        {
            this.coinPrefab = coinPrefab;

            for (int i = 0; i < initialCount; i++)
            {
                CreateNewCoin(Vector3.zero);
            }
        }

        public CoinController GetCoin(Vector3 spawnPos)
        {
            PooledCoin pooled = coins.Find(c => !c.isUsed);

            if (pooled != null)
            {
                pooled.isUsed = true;
                pooled.Controller.ResetCoin(coinPrefab, spawnPos);
                return pooled.Controller;
            }

            return CreateNewCoin(spawnPos);
        }

        public void ReturnCoinToPool(CoinController returnedCoin)
        {
            PooledCoin pooled = coins.Find(c => c.Controller == returnedCoin);

            if (pooled != null)
                pooled.isUsed = false;
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
