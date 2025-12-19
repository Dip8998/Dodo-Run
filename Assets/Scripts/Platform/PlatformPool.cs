using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public sealed class PlatformPool
    {
        private readonly Stack<PlatformController> free = new();
        private readonly List<PlatformController> all = new();
        private readonly PlatformScriptableObject data;

        public PlatformPool(PlatformScriptableObject data)
        {
            this.data = data;
        }

        public PlatformController Get(Vector3 pos, int index)
        {
            if (free.Count > 0)
            {
                var p = free.Pop();
                p.ResetPlatform(pos, index);
                return p;
            }

            var created = new PlatformController(data, pos, index);
            all.Add(created);
            return created;
        }

        public void Return(PlatformController platform)
        {
            free.Push(platform);
        }

        public void Update()
        {
            for (int i = 0; i < all.Count; i++)
                all[i].UpdatePlatform();
        }
    }
}
