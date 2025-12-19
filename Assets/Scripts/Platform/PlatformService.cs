using UnityEngine;

namespace DodoRun.Platform
{
    public sealed class PlatformService
    {
        public PlatformScriptableObject PlatformScriptableObject { get; }
        private readonly PlatformPool pool;

        private int counter;
        private PlatformController latest;

        public PlatformService(PlatformScriptableObject data, Vector3 spawn)
        {
            PlatformScriptableObject = data;
            pool = new PlatformPool(data);
            latest = CreatePlatform(spawn);
        }

        public void UpdatePlatform() => pool.Update();

        public PlatformController CreatePlatform(Vector3 pos)
        {
            counter++;
            latest = pool.Get(pos, counter);
            return latest;
        }

        public PlatformController GetLatestPlatform() => latest;

        public void ReturnPlatformToPool(PlatformController controller)
        {
            pool.Return(controller);
        }
    }
}
