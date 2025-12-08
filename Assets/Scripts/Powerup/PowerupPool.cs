using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.PowerUps
{
    public class PowerupPool
    {
        private readonly List<PowerupController> all = new List<PowerupController>();
        private readonly Stack<PowerupController> free = new Stack<PowerupController>();
        private readonly PowerupView prefab;

        public PowerupPool(PowerupView prefab)
        {
            this.prefab = prefab;
        }

        public PowerupController Get(PowerupType type, Vector3 position)
        {
            if (free.Count > 0)
            {
                PowerupController controller = free.Pop();
                controller.Reset(prefab, type, position);
                return controller;
            }

            PowerupController created = new PowerupController(prefab, type, position);
            all.Add(created);
            return created;
        }

        public void Return(PowerupController controller)
        {
            if (!free.Contains(controller))
                free.Push(controller);
        }
    }
}
