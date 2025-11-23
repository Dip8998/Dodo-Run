using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformService
    {
        public PlatformScriptableObject PlatformScriptableObject { get; private set; }
        private PlatformPool platformPool;

        public PlatformService(PlatformScriptableObject data, Vector3 spawnPos)
        {
            PlatformScriptableObject = data;
            platformPool = new PlatformPool(PlatformScriptableObject);
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
