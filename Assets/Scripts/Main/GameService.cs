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
        public DifficultyManager Difficulty { get; private set; }

        public bool IsGameRunning { get; private set; } = true;

        [SerializeField] private PlatformScriptableObject platformScriptableObject;
        [SerializeField] private PlayerScriptableObject playerScriptableObject;

        [Header("Obstacle Prefabs")]
        [SerializeField] private ObstacleView jumpObstaclePrefab;
        [SerializeField] private ObstacleView slideObstaclePrefab;
        [SerializeField] private ObstacleView slideOrJumpObstaclePrefab;
        [SerializeField] private ObstacleView trainObstaclePrefab;

        [Header("Coin Settings")]
        [SerializeField] private CoinView coinPrefab;
        [SerializeField] private float coinBaseVerticalOffset = 0.5f;

        [Header("Difficulty Settings")]
        [SerializeField] private DifficultySettings difficultySettings = new DifficultySettings();

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            EventService = new EventService();
            PlayerService = new PlayerService(playerScriptableObject);

            Difficulty = new DifficultyManager(difficultySettings);

            ObstacleService = new ObstacleService(jumpObstaclePrefab, slideObstaclePrefab, slideOrJumpObstaclePrefab, trainObstaclePrefab);

            CoinService = new CoinService(
                coinPrefab,
                coinBaseVerticalOffset
            );

            PlatformService = new PlatformService(
                platformScriptableObject,
                platformScriptableObject.spawnPosition
            );
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

    [System.Serializable]
    public class DifficultySettings
    {
        public float StartObstacleProbability = 0.30f;
        public float MaxObstacleProbability = 0.85f;

        public float StartHardObstacleChance = 0.10f;
        public float MaxHardObstacleChance = 0.60f;

        public float StartSpeed = 13f;
        public float MaxSpeed = 28f;

        public float DifficultyRampTime = 120f;
    }

    public class DifficultyManager
    {
        private DifficultySettings settings;
        private float startTime;

        public DifficultyManager(DifficultySettings settings)
        {
            this.settings = settings;
            startTime = Time.time;
        }

        public float Progress =>
            Mathf.Clamp01((Time.time - startTime) / settings.DifficultyRampTime);

        public float CurrentObstacleProbability =>
            Mathf.Lerp(settings.StartObstacleProbability, settings.MaxObstacleProbability, Progress);

        public float CurrentHardObstacleChance =>
            Mathf.Lerp(settings.StartHardObstacleChance, settings.MaxHardObstacleChance, Progress);

        public float CurrentSpeed =>
            Mathf.Lerp(settings.StartSpeed, settings.MaxSpeed, Progress);
    }
}