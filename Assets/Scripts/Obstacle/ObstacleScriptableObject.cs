using UnityEngine;

namespace DodoRun.Obstacle
{
    [CreateAssetMenu(fileName = "ObstacleSO", menuName = "ScriptableObject/ObstacleSO")]

    public class ObstacleScriptableObject : ScriptableObject
    {
        public ObstacleView[] Obstacles;
        public float ObstacleSegmentLength;
        public float SpawnProbability;
    }
}
