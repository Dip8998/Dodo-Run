using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformPool
    {
        private List<PooledPlateform> platforms = new List<PooledPlateform>();
        private PlatformScriptableObject platformData;

        public PlatformPool(PlatformScriptableObject platformData)
        {
            this.platformData = platformData;
        }

        public void UpatePlatformPool()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].Platform.UpdatePlatform();
            }
        }

        public PlatformController GetPlatform(Vector3 spawnPos, int platformIndex)
        {
            PooledPlateform platform = platforms.Find(item => !item.isUsed);

            if (platform != null)
            {
                platform.isUsed = true;
                platform.Platform.ResetPlatform(spawnPos, platformIndex);
                return platform.Platform;
            }
            return CreateNewPlatformPool(spawnPos, platformIndex);

        }

        public void ReturnPlatformToPool(PlatformController returnedPlatform)
        {
            PooledPlateform platform = platforms.Find(item => item.Platform.Equals(returnedPlatform));

            if (platform != null)
            {
                platform.isUsed = false;
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