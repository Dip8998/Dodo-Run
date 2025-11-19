namespace DodoRun.Player
{
    public class PlayerService
    {
        private PlayerController playerController;
        
        public PlayerService(PlayerScriptableObject playerScriptableObject)
        {
            playerController = new PlayerController(playerScriptableObject);
        }

        public void UpdatePlayer()
        {
            playerController.UpdatePlayer();
        }

        public void FixedUpdatePlayer()
        {
            playerController.FixedUpdatePlayer();
        }
    }
}
