using System.Collections.Generic;
using UnityEngine;
using DodoRun.Main;

namespace DodoRun.PowerUps
{
    public class PowerupService
    {
        private readonly PowerupPool magnetPool;
        private readonly PowerupPool shieldPool;

        private readonly List<PowerupController> active = new List<PowerupController>();
        private readonly Dictionary<PowerupType, float> activeTimers = new Dictionary<PowerupType, float>();

        private Transform player;
        private float despawnDistance = 7f;

        public float MagnetRange = 4f;

        public bool IsMagnetActive => activeTimers.ContainsKey(PowerupType.Magnet);
        public bool IsShieldActive => activeTimers.ContainsKey(PowerupType.Shield);

        public PowerupService(PowerupView magnet, PowerupView shield)
        {
            if (magnet != null)
                magnetPool = new PowerupPool(magnet);

            if (shield != null)
                shieldPool = new PowerupPool(shield);
        }

        public PowerupController Spawn(PowerupType type, Vector3 position)
        {
            PowerupController controller = null;

            if (type == PowerupType.Magnet && magnetPool != null)
                controller = magnetPool.Get(type, position);

            else if (type == PowerupType.Shield && shieldPool != null)
                controller = shieldPool.Get(type, position);

            if (controller != null)
                active.Add(controller);

            return controller;
        }

        public void ReturnToPool(PowerupController controller)
        {
            if (controller == null)
                return;

            active.Remove(controller);

            if (controller.Type == PowerupType.Magnet) magnetPool?.Return(controller);
            else if (controller.Type == PowerupType.Shield) shieldPool?.Return(controller);
        }

        public void ActivatePowerup(PowerupType type)
        {
            float duration = type switch
            {
                PowerupType.Magnet => 10f,
                PowerupType.Shield => 12f,
                _ => 8f
            };

            activeTimers[type] = Time.time + duration;
        }

        public void Update()
        {
            if (!GameService.Instance.IsGameRunning) return;

            if (player == null)
                player = GameService.Instance.PlayerService.GetPlayerTransform();

            if (player == null) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 movement = new Vector3(0, 0, -speed * Time.deltaTime);
            float playerZ = player.position.z;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                var controller = active[i];
                if (controller.View == null || !controller.View.gameObject.activeInHierarchy)
                {
                    active.RemoveAt(i);
                    continue;
                }

                var pos = controller.View.transform.position + movement;
                controller.View.transform.position = pos;

                if (pos.z < playerZ - despawnDistance)
                    controller.Deactivate();
            }

            var toRemove = new List<PowerupType>();
            foreach (var kvp in activeTimers)
            {
                if (Time.time >= kvp.Value)
                    toRemove.Add(kvp.Key);
            }

            foreach (var t in toRemove)
                activeTimers.Remove(t);
        }
    }
}
