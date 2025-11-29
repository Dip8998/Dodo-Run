using UnityEngine;

namespace DodoRun.Obstacle
{
	public class ObstacleView : MonoBehaviour
	{
        public float Height => GetComponent<Collider>().bounds.size.y;

        private ObstacleController obstacleController;

		public void SetController(ObstacleController obstacleController)
		{
			this.obstacleController = obstacleController;
		}
	}
}
