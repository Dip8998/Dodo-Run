using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstaclePool
    {
        private ObstacleScriptableObject obstacleData;
        private Dictionary<ObstacleView, List<ObstacleController>> pools = new Dictionary<ObstacleView, List<ObstacleController>>();

        public ObstaclePool(ObstacleScriptableObject data)
        {
            this.obstacleData = data;
        }

        public ObstacleController GetObstacle(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            if (!pools.ContainsKey(prefab))
            {
                pools.Add(prefab, new List<ObstacleController>());
            }

            ObstacleController obstacle = pools[prefab].Find(item => !item.IsUsed());

            if (obstacle != null)
            {
                obstacle.ResetObstacle(prefab, spawnPos, parent);
                return obstacle;
            }

            ObstacleController newObstacle = new ObstacleController(prefab, spawnPos, parent);
            pools[prefab].Add(newObstacle);
            return newObstacle;
        }

        public void ReturnObstacleToPool(ObstacleController returnedObstacle)
        {
            if (returnedObstacle != null)
            {
                returnedObstacle.Deactivate();
            }
        }
    }
}