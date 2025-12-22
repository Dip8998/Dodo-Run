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
using DodoRun.Data;
using System.Collections;

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

        public bool IsInitialized { get; private set; }
        public bool IsGameRunning { get; set; }

        [Header("Config")]
        [SerializeField] private PlatformScriptableObject platformData;
        [SerializeField] private PlayerScriptableObject playerData;

        [Header("Difficulty")]
        [SerializeField] private DifficultySettings difficultySettings;

        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverPanel;

        private GameLoop gameLoop;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            StartCoroutine(InitializeAsync());
        }

        private IEnumerator InitializeAsync()
        {
            IsGameRunning = false; 

            EventService = new EventService();
            Difficulty = new DifficultyManager(difficultySettings);
            PlayerService = new PlayerService(playerData);
            ScoreService = new ScoreService();

            ObstacleService = new ObstacleService();
            CoinService = new CoinService(0.5f);
            PowerupService = new PowerupService();

            var obstacleTask = ObstacleService.PreloadAssets();
            var coinTask = CoinService.Initialize();
            var powerupTask = PowerupService.Initialize();

            while (!obstacleTask.IsCompleted || !coinTask.IsCompleted || !powerupTask.IsCompleted)
            {
                yield return null;
            }

            if (obstacleTask.IsFaulted || coinTask.IsFaulted)
            {
                Debug.LogError("Critical Error: Addressables failed to load. Check console for details.");
                yield break;
            }

            TutorialService = new TutorialService();
            PlatformService = new PlatformService(platformData, platformData.spawnPosition);
            gameLoop = new GameLoop(this);

            IsGameRunning = true;
            IsInitialized = true;
            TutorialService.StartTutorial();
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

            int finalScore = ScoreService.TotalScore;
            PlayerDataService.TrySetHighScore(finalScore);

           StartCoroutine(SetActiveGameOverPanel());
        }

        IEnumerator SetActiveGameOverPanel()
        {
            yield return new WaitForSeconds(2f);
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }
}
