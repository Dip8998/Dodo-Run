using System.Collections.Generic;
using UnityEngine;
using DodoRun.Main;

namespace DodoRun.PowerUps
{
    public class PowerupService
    {
        private readonly PowerupView magnetPrefab;
        private readonly PowerupView shieldPrefab;

        private readonly PowerupPool magnetPool;
        private readonly PowerupPool shieldPool;

        private readonly List<PowerupController> active = new List<PowerupController>();

        private Transform player;
        private float despawnDistance = 7f;

        private PowerupType activePowerup = PowerupType.None;
        private float powerupEndTime;

        public float MagnetRange = 4f;

        public bool IsMagnetActive => activePowerup == PowerupType.Magnet && Time.time < powerupEndTime;
        public bool IsShieldActive => activePowerup == PowerupType.Shield && Time.time < powerupEndTime;

        public PowerupService(PowerupView magnet, PowerupView shield)
        {
            magnetPrefab = magnet;
            shieldPrefab = shield;

            if (magnetPrefab != null)
                magnetPool = new PowerupPool(magnetPrefab);
            if (shieldPrefab != null)
                shieldPool = new PowerupPool(shieldPrefab);
        }

        public PowerupController Spawn(PowerupType type, Vector3 position)
        {
            if (type == PowerupType.None)
                return null;

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

            if (active.Contains(controller))
                active.Remove(controller);

            if (controller.Type == PowerupType.Magnet && magnetPool != null)
                magnetPool.Return(controller);
            else if (controller.Type == PowerupType.Shield && shieldPool != null)
                shieldPool.Return(controller);
        }

        public void ActivatePowerup(PowerupType type)
        {
            activePowerup = type;

            float duration;

            if (type == PowerupType.Magnet)
                duration = 10f;
            else if (type == PowerupType.Shield)
                duration = 12f;
            else
                duration = 8f;

            powerupEndTime = Time.time + duration;
        }


        public void Update()
        {
            if (!GameService.Instance.IsGameRunning)
                return;

            if (player == null)
                player = GameService.Instance.PlayerService.GetPlayerTransform();

            if (player == null)
                return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            float delta = Time.deltaTime;
            Vector3 movement = new Vector3(0f, 0f, -speed * delta);

            float playerZ = player.position.z;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                PowerupController controller = active[i];
                if (controller == null || controller.View == null)
                {
                    active.RemoveAt(i);
                    continue;
                }

                Transform t = controller.View.transform;
                if (!t.gameObject.activeInHierarchy)
                {
                    active.RemoveAt(i);
                    continue;
                }

                Vector3 pos = t.position;
                pos += movement;
                t.position = pos;

                if (pos.z < playerZ - despawnDistance)
                    controller.Deactivate();
            }

            if (activePowerup != PowerupType.None && Time.time >= powerupEndTime)
                activePowerup = PowerupType.None;
        }
    }
}
