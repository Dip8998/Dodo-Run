using DodoRun.Platform;
using DodoRun.Utilities;
using UnityEngine;

namespace DodoRun.Main
{
    public class GameService : GenericMonoSingleton<GameService>
    {
        public PlatformService PlatformService { get; private set; }

        [SerializeField] private PlatformScriptableObject platformScriptableObject;
        [SerializeField] private Vector3 platformSpawnPosition;

        protected override void Awake()
        {
            base.Awake();
            PlatformService = new PlatformService(platformScriptableObject, platformSpawnPosition);
        }

        private void Update()
        {
            PlatformService.UpdatePlatform();
        }
    }
}
