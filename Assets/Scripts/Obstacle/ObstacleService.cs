using DodoRun.Main;
using DodoRun.Platform;
using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstacleService
    {
        public ObstacleScriptableObject ObstacleScriptableObject { get; private set; }
        private ObstaclePool obstaclePool;

        public ObstacleService(ObstacleScriptableObject data)
        {
            ObstacleScriptableObject = data;
            obstaclePool = new ObstaclePool(data);
        }

        public List<ObstacleController> SpawnRandomPattern(Vector3 segmentStartPos, float laneOffset, Transform parent)
        {
            if (ObstacleScriptableObject.Patterns.Length == 0) return null;

            int patternIndex = Random.Range(0, ObstacleScriptableObject.Patterns.Length);
            ObstaclePatternScriptableObject selectedPattern = ObstacleScriptableObject.Patterns[patternIndex];

            List<ObstacleController> spawned = new List<ObstacleController>();

            foreach (var patternObstacle in selectedPattern.ObstaclePositions)
            {
                float laneX = (int)patternObstacle.Lane * laneOffset;
                float obstacleYOffeset = 0.25f; 

                Vector3 spawnPosition = new Vector3(
                    segmentStartPos.x + laneX,
                    segmentStartPos.y + obstacleYOffeset,
                    segmentStartPos.z + patternObstacle.ZOffset 
                );

                ObstacleController controller = obstaclePool.GetObstacle(patternObstacle.ObstaclePrefab, spawnPosition, parent);
                spawned.Add(controller);
            }
            return spawned; 
        }
        public void ReturnObstacleToPool(ObstacleController controller)
        {
            obstaclePool.ReturnObstacleToPool(controller);
        }
    }
}