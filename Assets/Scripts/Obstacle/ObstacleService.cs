using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using DodoRun.Main;

namespace DodoRun.Obstacle
{
    public class ObstacleService
    {
        private readonly Dictionary<ObstacleType, string> addressKeys = new()
        {
            { ObstacleType.JumpOnly, "Obstacle_Jump" },
            { ObstacleType.SlideOnly, "Obstacle_Slide" },
            { ObstacleType.SlideOrJump, "Obstacle_Combo" },
            { ObstacleType.Train, "Obstacle_Train" }
        };

        private readonly Dictionary<string, ObstacleView> prefabCache = new();
        private readonly ObstaclePool obstaclePool;
        private readonly List<ObstacleController> activeObstacles = new();
        private readonly Queue<ObstacleType> lastObstacleHistory = new();
        private readonly Queue<int> lastLaneHistory = new();
        private const int historyLimit = 3;

        public ObstacleService()
        {
            obstaclePool = new ObstaclePool();
        }

        public async Task PreloadAssets()
        {
            List<Task> loadingTasks = new List<Task>();
            foreach (var kvp in addressKeys)
            {
                loadingTasks.Add(LoadAndCache(kvp.Key, kvp.Value));
            }
            await Task.WhenAll(loadingTasks);
            Debug.Log("ObstacleService: All assets preloaded successfully.");
        }

        private async Task LoadAndCache(ObstacleType type, string key)
        {
            if (prefabCache.ContainsKey(key)) return;

            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(key);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    prefabCache[key] = handle.Result.GetComponent<ObstacleView>();
                }
                else
                {
                    Debug.LogError($"Failed to load Addressable: {key} for {type}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception loading {key}: {e.Message}");
            }
        }

        public ObstacleController SpawnObstacle(ObstacleType type, int lane, Vector3 basePos, float laneOffset)
        {
            if (type == ObstacleType.None || !addressKeys.ContainsKey(type)) return null;

            string key = addressKeys[type];
            if (!prefabCache.TryGetValue(key, out var prefab))
            {
                Debug.LogError($"Obstacle {type} not preloaded!");
                return null;
            }

            Vector3 spawnPos = new Vector3(basePos.x + lane * laneOffset, basePos.y, basePos.z);
            ObstacleController controller = obstaclePool.GetObstacle(prefab, spawnPos, null);
            activeObstacles.Add(controller);
            return controller;
        }

        public void ReturnObstacleToPool(ObstacleController controller)
        {
            if (controller == null) return;
            activeObstacles.Remove(controller);
            obstaclePool.ReturnObstacleToPool(controller);
        }

        public ObstacleType GetBalancedRandomObstacleType()
        {
            ObstacleType type;
            do { type = GetRandomObstacleType(); }
            while (IsRepeatingType(type));
            AddObstacleHistory(type);
            return type;
        }

        private ObstacleType GetRandomObstacleType()
        {
            float p = GameService.Instance.Difficulty.Progress;
            float r = Random.value;
            if (r < Mathf.Lerp(0.6f, 0.35f, p)) return ObstacleType.JumpOnly;
            if (r < Mathf.Lerp(0.85f, 0.65f, p)) return ObstacleType.SlideOnly;
            return ObstacleType.SlideOrJump;
        }

        private bool IsRepeatingType(ObstacleType type)
        {
            if (lastObstacleHistory.Count < 2) return false;
            ObstacleType[] arr = lastObstacleHistory.ToArray();
            return arr[arr.Length - 1] == type && arr[arr.Length - 2] == type;
        }

        private void AddObstacleHistory(ObstacleType type)
        {
            lastObstacleHistory.Enqueue(type);
            if (lastObstacleHistory.Count > historyLimit) lastObstacleHistory.Dequeue();
        }

        public void DisableAllObstacleCollisions()
        {
            foreach (var obs in activeObstacles) obs.SetCollisionEnabled(false);
        }

        public void EnableAllObstacleCollisions()
        {
            foreach (var obs in activeObstacles) obs.SetCollisionEnabled(true);
        }
    }
}