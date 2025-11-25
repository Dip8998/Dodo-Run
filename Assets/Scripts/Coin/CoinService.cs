using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DodoRun.Coin
{
    public class CoinService
    {
        public CoinScriptableObject CoinScriptableObject { get; private set; }
        private CoinPool coinPool;
        private List<CoinController> activeCoins = new List<CoinController>();

        public CoinService(CoinScriptableObject coinScriptableObject)
        {
            CoinScriptableObject = coinScriptableObject;
            coinPool = new CoinPool(CoinScriptableObject);
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

        public List<CoinController> SpawnRandomCoinPattern(Vector3 segmentStartPos, float laneOffset, Transform parent)
        {
            if (CoinScriptableObject.RandomCoinPatterns == null || CoinScriptableObject.RandomCoinPatterns.Length == 0)
                return null;

            int patternIndex = Random.Range(0, CoinScriptableObject.RandomCoinPatterns.Length);
            CoinPatternScriptableObject selectedPattern = CoinScriptableObject.RandomCoinPatterns[patternIndex];

            return SpawnPatternInternal(selectedPattern, segmentStartPos, laneOffset, parent);
        }

        public List<CoinController> SpawnContextualCoinPattern(Vector3 segmentStartPos, float laneOffset, Transform parent, int obstacleContextTag)
        {
            if (CoinScriptableObject.ContextualCoinPatterns == null || CoinScriptableObject.ContextualCoinPatterns.Length == 0)
                return null;

            CoinPatternScriptableObject[] validPatterns =
                CoinScriptableObject.ContextualCoinPatterns.Where(p => p.ContextTag == obstacleContextTag).ToArray();

            if (validPatterns.Length == 0)
            {
                return null;
            }

            int patternIndex = Random.Range(0, validPatterns.Length);
            CoinPatternScriptableObject selectedPattern = validPatterns[patternIndex];

            return SpawnPatternInternal(selectedPattern, segmentStartPos, laneOffset, parent);
        }

        private List<CoinController> SpawnPatternInternal(CoinPatternScriptableObject pattern, Vector3 segmentStartPos, float laneOffset, Transform parent)
        {
            List<CoinController> spawned = new List<CoinController>();

            float globalYOffset = CoinScriptableObject.VerticalOffset;

            foreach (var coinPositionData in pattern.CoinPositions)
            {
                float laneX = (int)coinPositionData.Lane * laneOffset;

                float finalYOffset = (coinPositionData.YOffsetOverride != 0f)
                    ? coinPositionData.YOffsetOverride
                    : globalYOffset;

                Vector3 spawnPosition = new Vector3(
                    segmentStartPos.x + laneX,
                    segmentStartPos.y + finalYOffset, 
                    segmentStartPos.z + coinPositionData.ZOffset
                );

                CoinController controller = GetCoin(spawnPosition);
                controller.CoinView.transform.SetParent(parent);
                spawned.Add(controller);
            }
            return spawned;
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