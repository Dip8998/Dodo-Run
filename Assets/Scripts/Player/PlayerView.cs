using UnityEngine;
using DodoRun.Main;
using DodoRun.Sound;

namespace DodoRun.Player
{
    public sealed class PlayerView : MonoBehaviour
    {
        private PlayerController controller;

        [SerializeField] private Transform groundCheckPosition;

        private int deadlyLayer;

        public Transform GroundCheckPosition => groundCheckPosition;

        public void SetController(PlayerController controller)
        {
            this.controller = controller;
            deadlyLayer = LayerMask.NameToLayer("DeadlyObstacle");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer != deadlyLayer)
                return;

            if (GameService.Instance.PowerupService?.IsShieldActive == true)
                return;

            controller.Die();
            GameService.Instance.GameOver();
        }

        public void PlayFootstep()
        {
            if (!GameService.Instance.IsGameRunning || controller == null)
            {
                AudioManager.Instance.SetRunningSoundActive(false);
                return;
            }

            if (controller.GetState() == PlayerState.RUNNING && controller.IsGrounded)
            {
                AudioManager.Instance.SetRunningSoundActive(true);
            }
            else
            {
                AudioManager.Instance.SetRunningSoundActive(false);
            }
        }
    }
}
