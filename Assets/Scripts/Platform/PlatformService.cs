using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformService
    {
        private List<PlatformController> platforms = new List<PlatformController>();
        private PlatformScriptableObject platformScriptableObject;

        public PlatformService(PlatformScriptableObject data, Vector3 spawnPos)
        {
            platformScriptableObject = data;
            AddPlatform(CreatePlatform(spawnPos));
        }

        public void UpdatePlatform()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].UpdatePlatform();
            }
        }

        public void AddPlatform(PlatformController controller)
        {
            platforms.Add(controller);
        }

        public PlatformController CreatePlatform(Vector3 spawnPos)
        {
            PlatformController controller = new PlatformController(platformScriptableObject, spawnPos);
            return controller;
        }
    }
}
