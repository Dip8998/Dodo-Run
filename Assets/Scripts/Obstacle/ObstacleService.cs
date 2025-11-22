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

        public ObstacleController SpawnRandomObstacle(Vector3 spawnPos, Transform parent)
        {
            if (ObstacleScriptableObject.Obstacles.Length == 0) return null;

            int randomIndex = Random.Range(0, ObstacleScriptableObject.Obstacles.Length);
            ObstacleView selectedPrefab = ObstacleScriptableObject.Obstacles[randomIndex];

            ObstacleController controller = obstaclePool.GetObstacle(selectedPrefab, spawnPos, parent);
            return controller;
        }

        public void ReturnObstacleToPool(ObstacleController controller)
        {
            obstaclePool.ReturnObstacleToPool(controller);
        }
    }
}