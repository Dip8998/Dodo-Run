using DodoRun.Coin;
using DodoRun.Event;
using DodoRun.Obstacle;
using DodoRun.Platform;
using DodoRun.Player;
using DodoRun.Utilities;
using UnityEngine;

namespace DodoRun.Main
{
    public class GameService : GenericMonoSingleton<GameService>
    {
        public PlatformService PlatformService { get; private set; }
        public PlayerService PlayerService { get; private set; }
        public EventService EventService { get; private set; }
        public ObstacleService ObstacleService { get; private set; }
        public CoinService CoinService { get; private set; }

        public bool IsGameRunning { get; private set; } = true;

        [SerializeField] private PlatformScriptableObject platformScriptableObject;
        [SerializeField] private PlayerScriptableObject playerScriptableObject;
        [SerializeField] private ObstacleScriptableObject obstacleScriptableObject;
        [SerializeField] private CoinScriptableObject coinScriptableObject; 

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            EventService = new EventService();
            PlayerService = new PlayerService(playerScriptableObject);
            ObstacleService = new ObstacleService(obstacleScriptableObject);
            CoinService = new CoinService(coinScriptableObject);
            PlatformService = new PlatformService(platformScriptableObject, platformScriptableObject.spawnPosition);
        }

        private void Update()
        {
            if (!IsGameRunning) return;

            PlatformService.UpdatePlatform();
            PlayerService.UpdatePlayer();
        }

        private void FixedUpdate()
        {
            if (!IsGameRunning) return;

            PlayerService.FixedUpdatePlayer();
        }

        public void GameOver()
        {
            if (!IsGameRunning) return;
            IsGameRunning = false;
            Debug.Log("Game Over! All platform movement has stopped.");
        }
    }
}
