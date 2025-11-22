using UnityEngine;

namespace DodoRun.Obstacle
{
	public class ObstacleView : MonoBehaviour
	{
		private ObstacleController obstacleController;

		public void SetController(ObstacleController obstacleController)
		{
			this.obstacleController = obstacleController;
		}
	}
}
