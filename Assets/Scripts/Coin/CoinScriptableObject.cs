using UnityEngine;

namespace DodoRun.Coin
{
    [CreateAssetMenu(fileName = "CoinSO", menuName = "ScriptableObjects/CoinSO")]
    public class CoinScriptableObject : ScriptableObject
    {
        public CoinView CoinPrefab;
        public float VerticalOffset = 0.5f;

        [Range(0f, 1f)]
        public float SpawnProbability = 0.5f;

        public CoinPatternScriptableObject[] RandomCoinPatterns;
        public CoinPatternScriptableObject[] ContextualCoinPatterns;
    }
}