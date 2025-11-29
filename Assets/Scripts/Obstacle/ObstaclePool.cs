using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstaclePool
    {
        private readonly Dictionary<ObstacleView, List<ObstacleController>> pools =
            new Dictionary<ObstacleView, List<ObstacleController>>();

        public ObstacleController GetObstacle(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            if (!pools.TryGetValue(prefab, out var list))
            {
                list = new List<ObstacleController>();
                pools.Add(prefab, list);
            }

            ObstacleController controller = list.Find(o => !o.IsUsed());

            if (controller != null)
            {
                controller.ResetObstacle(prefab, spawnPos, parent);
                return controller;
            }

            ObstacleController newController = new ObstacleController(prefab, spawnPos, parent);
            list.Add(newController);
            return newController;
        }

        public void ReturnObstacleToPool(ObstacleController returnedObstacle)
        {
            returnedObstacle?.Deactivate();
        }
    }
}
