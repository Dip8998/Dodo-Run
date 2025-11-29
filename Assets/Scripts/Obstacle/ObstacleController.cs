using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstacleController
    {
        public ObstacleView ObstacleView { get; private set; }

        private bool isUsed = true;

        public ObstacleController(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            SetupView(prefab, spawnPos, parent);
        }

        private void SetupView(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            ObstacleView = Object.Instantiate(prefab, spawnPos, Quaternion.identity, parent);
            ObstacleView.SetController(this);
            isUsed = true;
        }

        public void ResetObstacle(ObstacleView prefab, Vector3 spawnPos, Transform parent)
        {
            if (ObstacleView == null)
            {
                SetupView(prefab, spawnPos, parent);
                return;
            }

            ObstacleView.transform.SetParent(parent);
            ObstacleView.transform.position = spawnPos;
            ObstacleView.gameObject.SetActive(true);
            isUsed = true;
        }

        public void Deactivate()
        {
            if (ObstacleView != null)
            {
                ObstacleView.gameObject.SetActive(false);
            }
            isUsed = false;
        }

        public bool IsUsed() => isUsed;
    }
}
