using UnityEngine;

namespace DodoRun.Obstacle
{
    [CreateAssetMenu(fileName = "ObstacleSO", menuName = "ScriptableObject/ObstacleSO")]

    public class ObstacleScriptableObject : ScriptableObject
    {
        public ObstaclePatternScriptableObject[] Patterns;
        public float ObstacleSegmentLength;
        public float SpawnProbability;
    }
}
