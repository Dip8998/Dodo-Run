using DodoRun.Platform;
using UnityEngine;

namespace DodoRun.Obstacle
{
    [System.Serializable]
    public class PatternObstacle
    {
        public PlatformLane Lane;
        public ObstacleView ObstaclePrefab;
        public float ZOffset;
        public int ContextTag = 0;
    }


    [CreateAssetMenu(fileName = "ObstaclePatternSO", menuName = "ScriptableObject/ObstaclePatternSO")]

    public class ObstaclePatternScriptableObject : ScriptableObject
    {
        public PatternObstacle[] ObstaclePositions;
        public int DifficultyRating;
    }
}
