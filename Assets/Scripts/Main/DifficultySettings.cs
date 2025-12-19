using UnityEngine;

namespace DodoRun.Main
{
    [System.Serializable]
    public class DifficultySettings
    {
        public float StartObstacleProbability = 0.30f;
        public float MaxObstacleProbability = 0.85f;

        public float StartHardObstacleChance = 0.10f;
        public float MaxHardObstacleChance = 0.60f;

        public float StartSpeed = 13f;
        public float MaxSpeed = 28f;

        public float DifficultyRampTime = 120f;
    }
}
