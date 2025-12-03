using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerView : MonoBehaviour
    {
        private PlayerController playerController;
        private const string obstaccleLayerName = "DeadlyObstacle";
        [SerializeField] private Transform groundCheckPosition;

        private int deadlyObstacleLayer;

        public Transform GroundCheckPosition => groundCheckPosition;

        public void SetController(PlayerController playerController)
        {
            this.playerController = playerController;
            deadlyObstacleLayer = LayerMask.NameToLayer(obstaccleLayerName);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == deadlyObstacleLayer)
            {
                playerController.Die();
                GameService.Instance.GameOver();
            }
        }

        private void OnDrawGizmos()
        {
            if (groundCheckPosition == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPosition.position, 0.08f);
        }
    }
}