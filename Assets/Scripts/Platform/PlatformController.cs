using DodoRun.Coin;
using DodoRun.Main;
using DodoRun.Obstacle;
using DodoRun.PowerUps;
using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Platform
{
    public sealed class PlatformController
    {
        public PlatformView PlatformView { get; private set; }

        private readonly PlatformScriptableObject data;
        private readonly List<ObstacleController> obstacles = new();
        private readonly List<CoinController> coins = new();

        private Transform transform;
        private Collider collider;

        private int platformIndex;
        private bool spawnedNext;

        private const float SegmentLength = 6f;
        private const int MinPowerupGapSegments = 2;
        private const int MaxPowerupGapSegments = 4;

        private static int segmentCounter;
        private static int nextPowerupSegment = 5;

        private enum Lane
        {
            Left = -1,
            Middle = 0,
            Right = 1
        }

        public PlatformController(
            PlatformScriptableObject data,
            Vector3 spawnPos,
            int index)
        {
            this.data = data;
            platformIndex = index;

            PlatformView = Object.Instantiate(data.Platform, spawnPos, Quaternion.identity);
            PlatformView.SetController(this);

            transform = PlatformView.transform;
            collider = PlatformView.GetComponent<Collider>();

            Initialize();
        }

        private void Initialize()
        {
            spawnedNext = false;

            if (GameService.Instance.TutorialService?.IsActive == true)
                return;

            SpawnProceduralContent();
        }

        public void UpdatePlatform()
        {
            if (!GameService.Instance.IsGameRunning) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 delta = Vector3.back * speed * Time.deltaTime;

            transform.position += delta;

            for (int i = 0; i < obstacles.Count; i++)
                obstacles[i].ObstacleView.transform.position += delta;
        }

        private void SpawnProceduralContent()
        {
            obstacles.Clear();
            coins.Clear();

            float laneOffset = data.LaneOffset;
            float length = data.PlatformLength;

            float backZ = transform.position.z - length * 0.5f;
            int segments = Mathf.FloorToInt(length / SegmentLength);
            int startSegment = Mathf.CeilToInt((data.SafeZoneDistance + 10f) / SegmentLength);

            for (int i = startSegment; i < segments; i++)
            {
                float z = backZ + i * SegmentLength + SegmentLength * 0.5f;
                Vector3 basePos = new(transform.position.x, transform.position.y, z);

                SpawnSegment(basePos, laneOffset);
            }
        }

        private void SpawnSegment(Vector3 basePos, float laneOffset)
        {
            float r = Random.value;

            bool leftTrain, middleTrain, rightTrain;
            bool leftObstacle, middleObstacle, rightObstacle;

            if (r < 0.6f)
            {
                int obstacleLane = Random.Range(0, 3);

                leftTrain = obstacleLane != 0;
                middleTrain = obstacleLane != 1;
                rightTrain = obstacleLane != 2;

                leftObstacle = obstacleLane == 0;
                middleObstacle = obstacleLane == 1;
                rightObstacle = obstacleLane == 2;
            }
            else
            {
                int trainLane = Random.Range(0, 3);

                leftTrain = trainLane == 0;
                middleTrain = trainLane == 1;
                rightTrain = trainLane == 2;

                leftObstacle = !leftTrain;
                middleObstacle = !middleTrain;
                rightObstacle = !rightTrain;
            }

            SpawnLane(basePos, laneOffset, Lane.Left, leftTrain, leftObstacle);
            SpawnLane(basePos, laneOffset, Lane.Middle, middleTrain, middleObstacle);
            SpawnLane(basePos, laneOffset, Lane.Right, rightTrain, rightObstacle);

            TrySpawnPowerup(
                ObstacleType.None,
                GetSafeLane(basePos, laneOffset),
                basePos,
                laneOffset
            );
        }

        private int GetSafeLane(Vector3 basePos, float laneOffset)
        {
            for (int lane = -1; lane <= 1; lane++)
            {
                bool blocked = false;

                for (int i = 0; i < obstacles.Count; i++)
                {
                    var view = obstacles[i].ObstacleView;
                    if (view == null || !view.gameObject.activeInHierarchy)
                        continue;

                    float dz = Mathf.Abs(view.transform.position.z - basePos.z);
                    if (dz > 6f)
                        continue;

                    float dx = Mathf.Abs(
                        view.transform.position.x -
                        (basePos.x + lane * laneOffset)
                    );

                    if (dx < laneOffset * 0.6f)
                    {
                        blocked = true;
                        break;
                    }
                }

                if (!blocked)
                    return lane;
            }

            return 0;
        }

        private void SpawnLane(
            Vector3 basePos,
            float laneOffset,
            Lane lane,
            bool spawnTrain,
            bool spawnObstacle)
        {
            var obstacleService = GameService.Instance.ObstacleService;

            int laneIndex = (int)lane;
            Vector3 spawnPos = basePos;

            if (spawnTrain)
            {
                spawnPos.y += 1.3f;

                var train = obstacleService.SpawnObstacle(
                    ObstacleType.Train,
                    laneIndex,
                    spawnPos,
                    laneOffset
                );

                if (train != null)
                    obstacles.Add(train);

                return;
            }

            if (spawnObstacle)
            {
                ObstacleType type = obstacleService.GetBalancedRandomObstacleType();
                spawnPos.y += 0.25f;

                var obs = obstacleService.SpawnObstacle(
                    type,
                    laneIndex,
                    spawnPos,
                    laneOffset
                );

                if (obs != null)
                    obstacles.Add(obs);

                SpawnCoins(type, laneIndex, basePos, laneOffset);
                return;
            }

            SpawnCoinLine(basePos, laneOffset, laneIndex, 6);
        }

        private void SpawnCoins(
            ObstacleType type,
            int lane,
            Vector3 basePos,
            float laneOffset)
        {
            if (type == ObstacleType.Train)
                return;

            int straight = 6;
            int arc = 12;

            switch (type)
            {
                case ObstacleType.JumpOnly:
                    SpawnCoinArc(basePos, laneOffset, lane, arc);
                    break;

                case ObstacleType.SlideOnly:
                    SpawnCoinsUnder(basePos, laneOffset, lane, straight);
                    break;

                case ObstacleType.SlideOrJump:
                    if (Random.value > 0.5f)
                        SpawnCoinArc(basePos, laneOffset, lane, arc);
                    else
                        SpawnCoinLine(basePos, laneOffset, lane, straight);
                    break;

                default:
                    SpawnCoinLine(basePos, laneOffset, lane, straight);
                    break;
            }
        }

        private void SpawnCoinLine(Vector3 basePos, float laneOffset, int lane, int count)
        {
            float y = transform.position.y +
                      GameService.Instance.CoinService.BaseVerticalOffset;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new(
                    basePos.x + lane * laneOffset,
                    y,
                    basePos.z + i * 1.5f
                );

                coins.Add(GameService.Instance.CoinService.GetCoin(pos));
            }
        }

        private void SpawnCoinsUnder(Vector3 basePos, float laneOffset, int lane, int count)
        {
            float y = transform.position.y + 0.5f;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new(
                    basePos.x + lane * laneOffset,
                    y,
                    basePos.z + i * 1.2f
                );

                coins.Add(GameService.Instance.CoinService.GetCoin(pos));
            }
        }

        private void SpawnCoinArc(Vector3 basePos, float laneOffset, int lane, int count)
        {
            float baseY = transform.position.y +
                          GameService.Instance.CoinService.BaseVerticalOffset;

            float arcLength = 10f;
            float start = -arcLength * 0.5f;
            float step = arcLength / (count - 1);

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                float height = Mathf.Sin(t * Mathf.PI) * 1.5f;

                Vector3 pos = new(
                    basePos.x + lane * laneOffset,
                    baseY + height,
                    basePos.z + start + step * i
                );

                coins.Add(GameService.Instance.CoinService.GetCoin(pos));
            }
        }

        private void TrySpawnPowerup(
            ObstacleType type,
            int lane,
            Vector3 basePos,
            float laneOffset)
        {
            var powerups = GameService.Instance.PowerupService;
            if (powerups == null) return;

            segmentCounter++;
            if (segmentCounter < nextPowerupSegment)
                return;

            Vector3 pos = new(
                basePos.x + lane * laneOffset,
                transform.position.y +
                GameService.Instance.CoinService.BaseVerticalOffset + 0.55f,
                basePos.z + SegmentLength * 2.5f 
            );

            float r = Random.value;

            PowerupType powerup =
                r < 0.45f ? PowerupType.Magnet :
                r < 0.80f ? PowerupType.Shield :
                            PowerupType.DoubleScore;

            powerups.Spawn(powerup, pos);

            segmentCounter = 0;
            nextPowerupSegment = Random.Range(
                MinPowerupGapSegments,
                MaxPowerupGapSegments + 1
            );
        }

        public void ResetPlatform(Vector3 spawnPos, int index)
        {
            platformIndex = index;
            spawnedNext = false;

            Cleanup();

            collider.enabled = false;
            transform.position = spawnPos;
            PlatformView.gameObject.SetActive(true);
            collider.enabled = true;

            Initialize();
        }

        private void Cleanup()
        {
            var game = GameService.Instance;

            for (int i = 0; i < obstacles.Count; i++)
                game.ObstacleService.ReturnObstacleToPool(obstacles[i]);

            obstacles.Clear();
        }

        public void HandleCollision(Collider other)
        {
            var game = GameService.Instance;

            if (other.CompareTag("Create") && !spawnedNext)
            {
                spawnedNext = true;

                Vector3 pos = transform.position;
                pos.z += data.PlatformLength - 0.2f;

                game.PlatformService.CreatePlatform(pos);
            }

            if (other.CompareTag("Destroy"))
            {
                Cleanup();
                game.PlatformService.ReturnPlatformToPool(this);
                PlatformView.gameObject.SetActive(false);
            }
        }

        public ObstacleController SpawnTutorialObstacle(
            ObstacleType type,
            int lane,
            Vector3 pos,
            float laneOffset)
        {
            var obs = GameService.Instance.ObstacleService
                .SpawnObstacle(type, lane, pos, laneOffset);

            if (obs != null)
                obstacles.Add(obs);

            return obs;
        }

        public void SpawnTutorialCoin(Vector3 pos)
        {
            coins.Add(GameService.Instance.CoinService.GetCoin(pos));
        }
    }
}
