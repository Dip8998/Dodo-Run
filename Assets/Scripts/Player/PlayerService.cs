using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerService
    {
        private PlayerController playerController;

        public PlayerService(PlayerScriptableObject playerScriptableObject)
        {
            playerController = new PlayerController(playerScriptableObject);
        }

        public void UpdatePlayer() => playerController.UpdatePlayer();

        public void FixedUpdatePlayer() => playerController.FixedUpdatePlayer();

        public float GetPlayerZ() => playerController.Rigidbody.transform.position.z;

        public Transform GetPlayerTransform() =>
            playerController?.Rigidbody?.transform;
    }
}
