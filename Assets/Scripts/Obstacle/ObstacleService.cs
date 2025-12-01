using System.Collections.Generic;
using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Obstacle
{
    public class ObstacleService
    {
        public ObstacleView JumpObstaclePrefab { get; private set; }
        public ObstacleView SlideObstaclePrefab { get; private set; }
        public ObstacleView SlideOrJumpObstaclePrefab { get; private set; }
        public ObstacleView TrainObstaclePrefab { get; private set; } 

        private readonly ObstaclePool obstaclePool;

        private readonly Queue<ObstacleType> lastObstacleHistory = new Queue<ObstacleType>();
        private readonly Queue<int> lastLaneHistory = new Queue<int>();
        private const int historyLimit = 3;

        public ObstacleService(
         ObstacleView jumpPrefab,
         ObstacleView slidePrefab,
         ObstacleView slideOrJumpPrefab,
         ObstacleView trainPrefab)                  
        {
            JumpObstaclePrefab = jumpPrefab;
            SlideObstaclePrefab = slidePrefab;
            SlideOrJumpObstaclePrefab = slideOrJumpPrefab;
            TrainObstaclePrefab = trainPrefab;

            obstaclePool = new ObstaclePool();
        }

        public ObstacleController SpawnObstacle(
            ObstacleType type,
            int lane,
            Vector3 basePos,
            float laneOffset)
        {
            if (type == ObstacleType.None)
                return null;

            ObstacleView prefab = GetPrefabByType(type);
            if (prefab == null)
                return null;

            Vector3 spawnPos = new Vector3(
                basePos.x + lane * laneOffset,
                basePos.y + 0.25f,
                basePos.z
            );

            ObstacleController controller =
                obstaclePool.GetObstacle(prefab, spawnPos, null);

            return controller;
        }

        public void ReturnObstacleToPool(ObstacleController controller)
        {
            if (controller != null)
            {
                obstaclePool.ReturnObstacleToPool(controller);
            }
        }

        public ObstacleType GetBalancedRandomObstacleType()
        {
            ObstacleType type;

            do
            {
                type = GetRandomObstacleType();
            }
            while (IsRepeatingType(type));

            AddObstacleHistory(type);
            return type;
        }

        private ObstacleType GetRandomObstacleType()
        {
            var difficulty = GameService.Instance.Difficulty;

            float obstacleProbability = difficulty.CurrentObstacleProbability;
            float hardChance = difficulty.CurrentHardObstacleChance;

            if (Random.value > obstacleProbability)
                return ObstacleType.None;

            bool spawnHard = Random.value < hardChance;

            if (spawnHard)
            {
                return (Random.value < 0.5f) ? ObstacleType.SlideOnly : ObstacleType.SlideOrJump;
            }
            else
            {
                return ObstacleType.JumpOnly;
            }
        }

        private bool IsRepeatingType(ObstacleType type)
        {
            if (type == ObstacleType.None) return false;
            if (lastObstacleHistory.Count < historyLimit) return false;

            foreach (ObstacleType t in lastObstacleHistory)
            {
                if (t != type) return false;
            }

            return true;
        }

        private void AddObstacleHistory(ObstacleType type)
        {
            lastObstacleHistory.Enqueue(type);
            if (lastObstacleHistory.Count > historyLimit)
                lastObstacleHistory.Dequeue();
        }

        public int GetBalancedLane()
        {
            int lane;
            do
            {
                lane = GetRandomLane();
            }
            while (IsRepeatingLane(lane));

            AddLaneHistory(lane);
            return lane;
        }

        private int GetRandomLane()
        {
            return Random.Range(-1, 2);
        }

        private bool IsRepeatingLane(int lane)
        {
            if (lastLaneHistory.Count < historyLimit) return false;

            foreach (int l in lastLaneHistory)
            {
                if (l != lane) return false;
            }

            return true;
        }

        private void AddLaneHistory(int lane)
        {
            lastLaneHistory.Enqueue(lane);
            if (lastLaneHistory.Count > historyLimit)
                lastLaneHistory.Dequeue();
        }

        private ObstacleView GetPrefabByType(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.JumpOnly:
                    return JumpObstaclePrefab;
                case ObstacleType.SlideOnly:
                    return SlideObstaclePrefab;
                case ObstacleType.SlideOrJump:
                    return SlideOrJumpObstaclePrefab;
                case ObstacleType.Train:                   
                    return TrainObstaclePrefab;
                default:
                    return null;
            }
        }
    }
}