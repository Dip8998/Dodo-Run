using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstacleController
    {
        public ObstacleView ObstacleView { get; private set; }
        private bool isUsed = true;

        public ObstacleController(ObstacleView obstacleView, Vector3 spawnPos, Transform parent)
        {
            ObstacleView = Object.Instantiate(obstacleView, spawnPos, Quaternion.identity, parent);
            isUsed = true;
        }

        public void ResetObstacle(ObstacleView obstacleView, Vector3 spawnPos, Transform parent)
        {
            ObstacleView.transform.SetParent(parent);
            ObstacleView.transform.position = spawnPos;
            ObstacleView.gameObject.SetActive(true);
            isUsed = true;
        }

        public void Deactivate()
        {
            ObstacleView.gameObject?.SetActive(false);
            isUsed = false;
        }

        public bool IsUsed() => isUsed;
    }
}
