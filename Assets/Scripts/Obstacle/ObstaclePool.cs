using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstaclePool
    {
        private readonly Dictionary<ObstacleView, List<ObstacleController>> pools =
            new Dictionary<ObstacleView, List<ObstacleController>>();

        private readonly Dictionary<ObstacleView, Stack<ObstacleController>> freeStacks =
            new Dictionary<ObstacleView, Stack<ObstacleController>>();

        public ObstacleController GetObstacle(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            if (!pools.ContainsKey(prefab))
            {
                pools[prefab] = new List<ObstacleController>();
                freeStacks[prefab] = new Stack<ObstacleController>();
            }

            var stack = freeStacks[prefab];

            if (stack.Count > 0)
            {
                ObstacleController controller = stack.Pop();
                controller.ResetObstacle(prefab, spawnPos, parent);
                return controller;
            }

            ObstacleController newController = new ObstacleController(prefab, spawnPos, parent);
            pools[prefab].Add(newController);
            return newController;
        }

        public void ReturnObstacleToPool(ObstacleController returnedObstacle)
        {
            if (returnedObstacle == null) return;

            returnedObstacle.Deactivate();

            foreach (var kvp in pools)
            {
                if (kvp.Value.Contains(returnedObstacle))
                {
                    freeStacks[kvp.Key].Push(returnedObstacle);
                    break;
                }
            }
        }
    }
}
