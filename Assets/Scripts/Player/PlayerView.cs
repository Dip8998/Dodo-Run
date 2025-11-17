using UnityEngine;

namespace DodoRun.Player
{
	public class PlayerView : MonoBehaviour
	{
		private PlayerController playerController;

		public void SetController(PlayerController playerController)
		{
			this.playerController = playerController;
		}
	}
}
