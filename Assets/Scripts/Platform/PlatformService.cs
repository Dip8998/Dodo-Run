using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformService
    {
        private PlatformScriptableObject platformScriptableObject;
        private PlatformPool platformPool;

        public PlatformService(PlatformScriptableObject data, Vector3 spawnPos)
        {
            platformScriptableObject = data;
            platformPool = new PlatformPool(platformScriptableObject);
            CreatePlatform(spawnPos);
        }

        public void UpdatePlatform()
        {
            platformPool.UpatePlatformPool();
        }

        public PlatformController CreatePlatform(Vector3 spawnPos)
        {
            PlatformController controller = platformPool.GetPlatform(spawnPos);
            return controller;
        }

        public void ReturnPlatformToPool(PlatformController controller)
        {
            platformPool.ReturnPlatformToPool(controller);
        }
    }
}
