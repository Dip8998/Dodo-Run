using UnityEngine;

namespace DodoRun.Platform
{
	public class PlatformView : MonoBehaviour
	{
		private PlatformController platformController;

		public void SetController(PlatformController platformController)
		{
			this.platformController = platformController;
		}

        private void OnTriggerEnter(Collider other)
        {
			platformController.HandleCollision(other);
        }
    }
}
