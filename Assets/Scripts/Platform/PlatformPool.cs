using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformPool
    {
        private List<PooledPlateform> platforms = new List<PooledPlateform>();
        private Stack<PooledPlateform> freePlatforms = new Stack<PooledPlateform>();
        private PlatformScriptableObject platformData;

        public PlatformPool(PlatformScriptableObject platformData)
        {
            this.platformData = platformData;
        }

        public void UpatePlatformPool()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].isUsed)
                {
                    platforms[i].Platform.UpdatePlatform();
                }
            }
        }

        public PlatformController GetPlatform(Vector3 spawnPos, int platformIndex)
        {
            if (freePlatforms.Count > 0)
            {
                PooledPlateform platform = freePlatforms.Pop();
                platform.isUsed = true;
                platform.Platform.ResetPlatform(spawnPos, platformIndex);
                return platform.Platform;
            }

            return CreateNewPlatformPool(spawnPos, platformIndex);
        }

        public void ReturnPlatformToPool(PlatformController returnedPlatform)
        {
            if (returnedPlatform == null) return;

            for (int i = 0; i < platforms.Count; i++)
            {
                PooledPlateform platform = platforms[i];
                if (platform.Platform == returnedPlatform && platform.isUsed)
                {
                    platform.isUsed = false;
                    freePlatforms.Push(platform);
                    break;
                }
            }
        }

        public PlatformController CreateNewPlatformPool(Vector3 spawnPos, int platformIndex)
        {
            PooledPlateform platform = new PooledPlateform();
            platform.Platform = new PlatformController(platformData, spawnPos, platformIndex);
            platform.isUsed = true;
            platforms.Add(platform);
            return platform.Platform;
        }

        public class PooledPlateform
        {
            public PlatformController Platform;
            public bool isUsed;
        }
    }
}