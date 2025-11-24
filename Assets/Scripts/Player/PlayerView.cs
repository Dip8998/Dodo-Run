using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerView : MonoBehaviour
    {
        private PlayerController playerController;
        [SerializeField] private Transform groundCheckPosition;

        private int deadlyObstacleLayer;

        public Transform GroundCheckPosition => groundCheckPosition;

        public void SetController(PlayerController playerController)
        {
            this.playerController = playerController;
            deadlyObstacleLayer = LayerMask.NameToLayer("DeadlyObstacle");
        }

        private void OnDrawGizmos()
        {
            if (GroundCheckPosition == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GroundCheckPosition.position, 0.08f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == deadlyObstacleLayer)
            {
                playerController.Die();
                GameService.Instance.GameOver();
                return;
            }
        }
    }
}