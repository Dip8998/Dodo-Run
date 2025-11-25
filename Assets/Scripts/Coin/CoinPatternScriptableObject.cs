using System;
using UnityEngine;
using DodoRun.Platform; 

namespace DodoRun.Coin
{
    [Serializable]
    public class CoinPositionData
    {
        public PlatformLane Lane;
        public float ZOffset = 0f;
        public float YOffsetOverride = 0f;
    }

    [CreateAssetMenu(fileName = "CoinPatternSO", menuName = "ScriptableObjects/CoinPatternSO")]
    public class CoinPatternScriptableObject : ScriptableObject
    {
        public CoinPositionData[] CoinPositions;
        public int ContextTag = 0;
    }
}