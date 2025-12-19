using System.Collections.Generic;

namespace DodoRun.PowerUps
{
    public sealed class PowerupPool
    {
        private readonly Stack<PowerupController> free = new();
        private readonly PowerupView prefab;

        public PowerupPool(PowerupView prefab)
        {
            this.prefab = prefab;
        }

        public PowerupController Get(PowerupType type, UnityEngine.Vector3 pos)
        {
            if (free.Count > 0)
            {
                var controller = free.Pop();
                controller.Initialize(prefab, type, pos);
                return controller;
            }

            return new PowerupController(prefab, type, pos);
        }

        public void Return(PowerupController controller)
        {
            free.Push(controller);
        }
    }
}
