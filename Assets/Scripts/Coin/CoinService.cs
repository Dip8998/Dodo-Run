using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using DodoRun.Main;

namespace DodoRun.Coin
{
    public class CoinService
    {
        private const string COIN_ADDRESS = "Entity_Coin";
        private CoinPool coinPool;
        private readonly List<CoinController> activeCoins = new();
        public float BaseVerticalOffset { get; private set; }
        public IReadOnlyList<CoinController> ActiveCoins => activeCoins;

        public CoinService(float verticalOffset)
        {
            BaseVerticalOffset = verticalOffset;
        }

        public async Task Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(COIN_ADDRESS);
            await handle.Task;
            coinPool = new CoinPool(handle.Result.GetComponent<CoinView>());
        }

        public CoinController GetCoin(Vector3 spawnPos)
        {
            if (coinPool == null) return null;
            CoinController coinController = coinPool.GetCoin(spawnPos);
            activeCoins.Add(coinController);
            return coinController;
        }

        public void ReturnCoinToPool(CoinController coinController)
        {
            if (coinController == null) return;
            activeCoins.Remove(coinController);
            coinPool.ReturnCoinToPool(coinController);
        }

        public void UpdateCoins()
        {
            if (!GameService.Instance.IsGameRunning) return;
            var player = GameService.Instance.PlayerService.GetPlayerTransform();
            if (player == null) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 delta = Vector3.back * speed * Time.deltaTime;

            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                var coin = activeCoins[i];
                coin.CoinView.transform.position += delta;
                if (coin.CoinView.transform.position.z < player.position.z - 5f)
                    coin.Deactivate();
            }
        }

        public void ReleaseAllMagnetCoins() => activeCoins.ForEach(c => c.IsBeingPulled = false);
    }
}