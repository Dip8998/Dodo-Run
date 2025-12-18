using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformService
    {
        public PlatformScriptableObject PlatformScriptableObject { get; private set; }
        private PlatformPool platformPool;
        private int platformCounter = 0;

        private PlatformController latestPlatform;

        public PlatformService(PlatformScriptableObject data, Vector3 spawnPos)
        {
            PlatformScriptableObject = data;
            platformPool = new PlatformPool(PlatformScriptableObject);
            latestPlatform = CreatePlatform(spawnPos);
        }

        public void UpdatePlatform()
        {
            platformPool.UpatePlatformPool();
        }

        public PlatformController CreatePlatform(Vector3 spawnPos)
        {
            platformCounter++;
            latestPlatform = platformPool.GetPlatform(spawnPos, platformCounter);
            return latestPlatform;
        }

        public PlatformController GetLatestPlatform()
        {
            return latestPlatform;
        }

        public void ReturnPlatformToPool(PlatformController controller)
        {
            platformPool.ReturnPlatformToPool(controller);
        }
    }
}
