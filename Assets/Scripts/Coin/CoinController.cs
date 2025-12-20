using UnityEngine;
using DodoRun.Main;
using DodoRun.Data;

namespace DodoRun.Coin
{
    public sealed class CoinController
    {
        public CoinView CoinView { get; private set; }
        public bool IsBeingPulled { get; set; }

        private bool isUsed;

        public CoinController(CoinView prefab, Vector3 spawnPos)
        {
            Create(prefab, spawnPos);
        }

        private void Create(CoinView prefab, Vector3 pos)
        {
            CoinView = Object.Instantiate(prefab, pos, Quaternion.identity);
            CoinView.SetController(this);
            isUsed = true;
        }

        public void Reset(CoinView prefab, Vector3 pos)
        {
            IsBeingPulled = false;

            if (CoinView == null)
            {
                Create(prefab, pos);
                return;
            }

            CoinView.transform.position = pos;
            CoinView.gameObject.SetActive(true);
            isUsed = true;
        }

        public void Collect()
        {
            GameService.Instance.ScoreService.AddCoinScore(10);
            GameService.Instance.ScoreService.AddCoins(1);

            PlayerDataService.AddCoins(1);

            GameService.Instance.EventService.OnCoinCollected.InvokeEvent(1);

            Deactivate();
        }

        public void Deactivate()
        {
            if (!isUsed) return;

            isUsed = false;
            CoinView.gameObject.SetActive(false);
            GameService.Instance.CoinService.ReturnCoinToPool(this);
        }

        public bool IsUsed() => isUsed;
    }
}
