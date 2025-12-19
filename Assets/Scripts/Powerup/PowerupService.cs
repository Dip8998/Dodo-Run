using System.Collections.Generic;
using UnityEngine;
using DodoRun.Main;
using UnityEngine.Pool;

namespace DodoRun.PowerUps
{
    public sealed class PowerupService
    {
        private readonly Dictionary<PowerupType, PowerupPool> pools = new();
        private readonly List<PowerupController> active = new();
        private readonly Dictionary<PowerupType, float> timers = new();

        private Transform player;

        private const float DespawnDistance = 7f;
        public float MagnetRange = 4f;

        public bool IsMagnetActive => timers.ContainsKey(PowerupType.Magnet);
        public bool IsShieldActive => timers.ContainsKey(PowerupType.Shield);
        public bool IsDoubleScoreActive => timers.ContainsKey(PowerupType.DoubleScore);

        public PowerupService(
            PowerupView magnet,
            PowerupView shield,
            PowerupView doubleScore)
        {
            if (magnet != null) pools[PowerupType.Magnet] = new PowerupPool(magnet);
            if (shield != null) pools[PowerupType.Shield] = new PowerupPool(shield);
            if (doubleScore != null) pools[PowerupType.DoubleScore] = new PowerupPool(doubleScore);
        }

        public PowerupController Spawn(PowerupType type, Vector3 pos)
        {
            if (!pools.TryGetValue(type, out var pool))
                return null;

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
            float duration = type switch
            {
                PowerupType.Magnet => 10f,
                PowerupType.Shield => 10f,
                PowerupType.DoubleScore => 10f,
                _ => 8f
            };

            timers[type] = Time.time + duration;
            GameService.Instance.EventService.OnPowerupActivated.InvokeEvent(type, duration);

            if (type == PowerupType.DoubleScore)
                GameService.Instance.ScoreService.ActivateDoubleScore();

            if (type == PowerupType.Shield)
                GameService.Instance.ObstacleService.DisableAllObstacleCollisions();
        }

        public void Update()
        {
            if (!GameService.Instance.IsGameRunning)
                return;

            if (player == null)
                player = GameService.Instance.PlayerService.GetPlayerTransform();

            if (player == null)
                return;

            MoveActivePowerups();
            HandleExpirations();
        }

        private void MoveActivePowerups()
        {
            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 delta = Vector3.back * speed * Time.deltaTime;
            float playerZ = player.position.z;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                var p = active[i];
                if (!p.View.gameObject.activeInHierarchy)
                {
                    active.RemoveAt(i);
                    continue;
                }

                p.View.transform.position += delta;

                if (p.View.transform.position.z < playerZ - DespawnDistance)
                    p.Deactivate();
            }
        }

        private void HandleExpirations()
        {
            if (timers.Count == 0)
                return;

            var expired = ListPool<PowerupType>.Get();

            foreach (var kvp in timers)
            {
                if (Time.time >= kvp.Value)
                    expired.Add(kvp.Key);
            }

            for (int i = 0; i < expired.Count; i++)
            {
                PowerupType type = expired[i];

                timers.Remove(type);
                GameService.Instance.EventService.OnPowerupExpired.InvokeEvent(type);

                if (type == PowerupType.DoubleScore)
                    GameService.Instance.ScoreService.DeactivateDoubleScore();

                if (type == PowerupType.Magnet)
                    GameService.Instance.CoinService.ReleaseAllMagnetCoins();
            }

            ListPool<PowerupType>.Release(expired);
        }
    }
}
