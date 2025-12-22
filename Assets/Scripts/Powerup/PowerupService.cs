using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using DodoRun.Main;
using DodoRun.Sound;

namespace DodoRun.PowerUps
{
    public sealed class PowerupService
    {
        private readonly Dictionary<PowerupType, string> addressKeys = new()
        {
            { PowerupType.Magnet, "Powerup_Magnet" },
            { PowerupType.Shield, "Powerup_Shield" },
            { PowerupType.DoubleScore, "Powerup_Double" }
        };

        private readonly Dictionary<PowerupType, PowerupPool> pools = new();
        private readonly List<PowerupController> active = new();
        private readonly Dictionary<PowerupType, float> timers = new();
        private Transform player;

        public bool IsMagnetActive => timers.ContainsKey(PowerupType.Magnet);
        public bool IsShieldActive => timers.ContainsKey(PowerupType.Shield);
        public float MagnetRange = 6f;

        public async Task Initialize()
        {
            foreach (var kvp in addressKeys)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(kvp.Value);
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    pools[kvp.Key] = new PowerupPool(handle.Result.GetComponent<PowerupView>());
                }
            }
        }

        public PowerupController Spawn(PowerupType type, Vector3 pos)
        {
            if (!pools.TryGetValue(type, out var pool)) return null;
            var controller = pool.Get(type, pos);
            active.Add(controller);
            return controller;
        }

        public void ReturnToPool(PowerupController controller)
        {
            active.Remove(controller);
            pools[controller.Type].Return(controller);
        }

        public void ActivatePowerup(PowerupType type)
        {
            float duration = 10f;
            timers[type] = Time.time + duration;
            GameService.Instance.EventService.OnPowerupActivated.InvokeEvent(type, duration);

            if (type == PowerupType.DoubleScore) GameService.Instance.ScoreService.ActivateDoubleScore();
            if (type == PowerupType.Shield) GameService.Instance.ObstacleService.DisableAllObstacleCollisions();
        }

        public void Update()
        {
            if (!GameService.Instance.IsGameRunning) return;
            if (player == null) player = GameService.Instance.PlayerService.GetPlayerTransform();
            if (player == null) return;

            MoveActivePowerups();
            HandleExpirations();
        }

        private void MoveActivePowerups()
        {
            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 delta = Vector3.back * speed * Time.deltaTime;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                active[i].View.transform.position += delta;
                if (active[i].View.transform.position.z < player.position.z - 7f)
                    active[i].Deactivate();
            }
        }

        private void HandleExpirations()
        {
            List<PowerupType> expired = new();
            foreach (var kvp in timers)
                if (Time.time >= kvp.Value) expired.Add(kvp.Key);

            foreach (var type in expired)
            {
                timers.Remove(type);
                AudioManager.Instance.PlayEffect(SoundType.Powerdown);
                GameService.Instance.EventService.OnPowerupExpired.InvokeEvent(type);
                if (type == PowerupType.DoubleScore) GameService.Instance.ScoreService.DeactivateDoubleScore();
                if (type == PowerupType.Shield) GameService.Instance.ObstacleService.EnableAllObstacleCollisions();
            }
        }
    }
}