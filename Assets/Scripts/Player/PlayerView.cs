using UnityEngine;
using DodoRun.Main;

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
    }
}
