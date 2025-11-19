using UnityEngine;

namespace DodoRun.Player
{
	public class PlayerView : MonoBehaviour
	{
		private PlayerController playerController;
		[SerializeField] private Transform groundCheckPosition;
		
		public Transform GroundCheckPosition => groundCheckPosition;

		public void SetController(PlayerController playerController)
		{
			this.playerController = playerController;
		}

        private void OnDrawGizmos()
        {
            if (GroundCheckPosition == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GroundCheckPosition.position, 0.08f); 
        }
    }
}
