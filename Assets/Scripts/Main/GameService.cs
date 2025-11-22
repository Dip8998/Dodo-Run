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

        [SerializeField] private PlatformScriptableObject platformScriptableObject;
        [SerializeField] private PlayerScriptableObject playerScriptableObject;
        [SerializeField] private ObstacleScriptableObject obstacleScriptableObject; 

        protected override void Awake()
        {
            base.Awake();
            EventService = new EventService();
            PlayerService = new PlayerService(playerScriptableObject);
            ObstacleService = new ObstacleService(obstacleScriptableObject);
            PlatformService = new PlatformService(platformScriptableObject, platformScriptableObject.spawnPosition);
        }

        private void Update()
        {
            PlatformService.UpdatePlatform();
            PlayerService.UpdatePlayer();
        }

        private void FixedUpdate()
        {
            PlayerService.FixedUpdatePlayer();
        }
    }
}
