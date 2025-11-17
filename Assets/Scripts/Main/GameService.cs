using DodoRun.Event;
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

        [SerializeField] private PlatformScriptableObject platformScriptableObject;
        [SerializeField] private PlayerScriptableObject playerScriptableObject;

        protected override void Awake()
        {
            base.Awake();
            EventService = new EventService();
            PlayerService = new PlayerService(playerScriptableObject);
            PlatformService = new PlatformService(platformScriptableObject, platformScriptableObject.spawnPosition);
        }

        private void Update()
        {
            PlatformService.UpdatePlatform();
            PlayerService.UpdatePlayer();
        }
    }
}
