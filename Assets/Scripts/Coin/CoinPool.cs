// CoinPool.cs
using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Coin
{
    public class CoinPool
    {
        private List<PooledCoin> coins = new List<PooledCoin>();
        private CoinScriptableObject coinData;

        public CoinPool(CoinScriptableObject coinData)
        {
            this.coinData = coinData;
            for (int i = 0; i < 10; i++)
            {
                CreateNewCoinPool(Vector3.zero);
            }
        }

        public CoinController GetCoin(Vector3 spawnPos)
        {
            PooledCoin pooledCoin = coins.Find(item => !item.isUsed);

            if (pooledCoin != null)
            {
                pooledCoin.isUsed = true;
                pooledCoin.Controller.ResetCoin(coinData.CoinPrefab, spawnPos);
                return pooledCoin.Controller;
            }

            return CreateNewCoinPool(spawnPos);
        }

        public void ReturnCoinToPool(CoinController returnedCoin)
        {
            PooledCoin pooledCoin = coins.Find(item => item.Controller.Equals(returnedCoin));

            if (pooledCoin != null)
            {
                pooledCoin.isUsed = false;
            }
        }

        private CoinController CreateNewCoinPool(Vector3 spawnPos)
        {
            PooledCoin pooledCoin = new PooledCoin();
            pooledCoin.Controller = new CoinController(coinData.CoinPrefab, spawnPos);

            pooledCoin.isUsed = true;
            coins.Add(pooledCoin);
            return pooledCoin.Controller;
        }

        public class PooledCoin
        {
            public CoinController Controller;
            public bool isUsed;
        }
    }
}