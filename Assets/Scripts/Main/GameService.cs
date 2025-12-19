using UnityEngine;
using DodoRun.Platform;
using DodoRun.Player;
using DodoRun.Obstacle;
using DodoRun.Coin;
using DodoRun.PowerUps;
using DodoRun.Score;
using DodoRun.Tutorial;
using DodoRun.Event;
using DodoRun.Utilities;
using TMPro;

namespace DodoRun.Main
{
    public sealed class GameService : GenericMonoSingleton<GameService>
    {
        public PlatformService PlatformService { get; private set; }
        public PlayerService PlayerService { get; private set; }
        public ObstacleService ObstacleService { get; private set; }
        public CoinService CoinService { get; private set; }
        public PowerupService PowerupService { get; private set; }
        public ScoreService ScoreService { get; private set; }
        public TutorialService TutorialService { get; private set; }
        public EventService EventService { get; private set; }
        public DifficultyManager Difficulty { get; private set; }

        public bool IsGameRunning { get; private set; }

        [Header("Config")]
        [SerializeField] private PlatformScriptableObject platformData;
        [SerializeField] private PlayerScriptableObject playerData;

        [Header("Obstacle Prefabs")]
        [SerializeField] private ObstacleView jumpObstacle;
        [SerializeField] private ObstacleView slideObstacle;
        [SerializeField] private ObstacleView slideOrJumpObstacle;
        [SerializeField] private ObstacleView trainObstacle;

        [Header("Coin")]
        [SerializeField] private CoinView coinPrefab;
        [SerializeField] private float coinVerticalOffset = 0.5f;

        [Header("Powerups")]
        [SerializeField] private PowerupView magnet;
        [SerializeField] private PowerupView shield;
        [SerializeField] private PowerupView doubleScore;

        [Header("Difficulty")]
        [SerializeField] private DifficultySettings difficultySettings;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI multiplierText;

        private GameLoop gameLoop;

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            Bootstrap();
        }

        private void Bootstrap()
        {
            EventService = new EventService();
            Difficulty = new DifficultyManager(difficultySettings);

            PlayerService = new PlayerService(playerData);

            ScoreService = new ScoreService();
            ScoreService.Initialize(scoreText, multiplierText);

            ObstacleService = new ObstacleService(
                jumpObstacle,
                slideObstacle,
                slideOrJumpObstacle,
                trainObstacle
            );

            CoinService = new CoinService(
                coinPrefab,
                coinVerticalOffset
            );

            PowerupService = new PowerupService(
                magnet,
                shield,
                doubleScore
            );

            TutorialService = new TutorialService();
            TutorialService.StartTutorial();

            PlatformService = new PlatformService(
                platformData,
                platformData.spawnPosition
            );

            gameLoop = new GameLoop(this);

            IsGameRunning = true;
        }

        private void Update()
        {
            if (!IsGameRunning) return;
            gameLoop.Tick();
        }

        private void FixedUpdate()
        {
            if (!IsGameRunning) return;
            gameLoop.FixedTick();
        }

        public void GameOver()
        {
            if (!IsGameRunning) return;
            IsGameRunning = false;
        }
    }
}
