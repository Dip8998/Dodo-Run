using System.Collections.Generic;
using DodoRun.Coin;
using DodoRun.Main;
using DodoRun.Obstacle;
using DodoRun.PowerUps;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformController
    {
        private PlatformScriptableObject platformData;
        public PlatformView PlatformView { get; private set; }

        private Rigidbody rigidbody;
        private Transform platformTransform;
        private Collider platformCollider;

        private bool hasSpawnedNext = false;
        private bool isDestroyed = false;
        private int platformIndex;

        private readonly List<ObstacleController> spawnedObstacles = new List<ObstacleController>();
        private readonly List<CoinController> spawnedCoins = new List<CoinController>();

        private const float SegmentLength = 10f;
        private int trainSegmentsLeft = 0;
        private int trainLane = 0;

        private static int segmentCounter = 0;
        private static int nextPowerupSegment = 5;

        public PlatformController(PlatformScriptableObject platformScriptableObject, Vector3 spawnPos, int index)
        {
            platformData = platformScriptableObject;
            platformIndex = index;
            SetupView(spawnPos);
        }

        private void SetupView(Vector3 spawnPos)
        {
            PlatformView = Object.Instantiate(platformData.Platform, spawnPos, Quaternion.identity);
            platformTransform = PlatformView.transform;
            rigidbody = PlatformView.GetComponent<Rigidbody>();
            platformCollider = PlatformView.GetComponent<Collider>();
            PlatformView.SetController(this);

            SpawnObstaclesAndCoinsProcedural();
        }

        public void UpdatePlatform()
        {
            if (isDestroyed || rigidbody == null) return;
            if (!GameService.Instance.IsGameRunning) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            float delta = Time.deltaTime;

            Vector3 movement = new Vector3(0f, 0f, -speed * delta);

            Vector3 platformPos = platformTransform.position;
            platformPos += movement;
            platformTransform.position = platformPos;

            for (int i = 0; i < spawnedObstacles.Count; i++)
            {
                ObstacleView view = spawnedObstacles[i].ObstacleView;
                if (view != null && view.gameObject.activeInHierarchy)
                {
                    Transform t = view.transform;
                    Vector3 pos = t.position;
                    pos += movement;
                    t.position = pos;
                }
            }
        }

        private void SpawnObstaclesAndCoinsProcedural()
        {
            spawnedObstacles.Clear();
            spawnedCoins.Clear();

            float platformLength = platformData.PlatformLength;
            float laneOffset = platformData.LaneOffset;

            float platformBackEdgeZ = platformTransform.position.z - (platformLength / 2f);
            int numberOfSegments = Mathf.FloorToInt(platformLength / SegmentLength);
            int startSegmentIndex = Mathf.CeilToInt((platformData.SafeZoneDistance + 10f) / SegmentLength);

            var gameService = GameService.Instance;
            var obstacleService = gameService.ObstacleService;
            var coinService = gameService.CoinService;

            for (int i = startSegmentIndex; i < numberOfSegments; i++)
            {
                float segmentZ =
                    platformBackEdgeZ +
                    (i * SegmentLength) +
                    (SegmentLength / 2f);

                Vector3 segmentBase = new Vector3(
                    platformTransform.position.x,
                    platformTransform.position.y,
                    segmentZ
                );

                ObstacleType obstacleType;
                int lane;

                if (trainSegmentsLeft > 0)
                {
                    obstacleType = ObstacleType.Train;
                    lane = trainLane;
                    trainSegmentsLeft--;
                }
                else
                {
                    if (platformIndex > 2 && ShouldStartTrainRun())
                    {
                        float p = gameService.Difficulty.Progress;

                        obstacleType = ObstacleType.Train;
                        lane = obstacleService.GetBalancedLane();
                        trainLane = lane;

                        int minTrainSize = Mathf.RoundToInt(Mathf.Lerp(2f, 4f, p));
                        int maxTrainSize = Mathf.RoundToInt(Mathf.Lerp(4f, 9f, p));

                        trainSegmentsLeft = Random.Range(minTrainSize, maxTrainSize);
                    }
                    else
                    {
                        obstacleType = obstacleService.GetBalancedRandomObstacleType();
                        lane = obstacleService.GetBalancedLane();
                    }
                }

                Vector3 spawnPos = segmentBase;

                if (obstacleType == ObstacleType.Train)
                {
                    spawnPos.y = platformTransform.position.y + 1.3f;
                }
                else
                {
                    spawnPos.y = platformTransform.position.y + 0.25f;
                }

                ObstacleController obstacle =
                    obstacleService.SpawnObstacle(
                        obstacleType,
                        lane,
                        spawnPos,
                        laneOffset
                    );

                if (obstacle != null)
                {
                    spawnedObstacles.Add(obstacle);
                }

                SpawnCoinsForSegment(obstacleType, lane, segmentBase, laneOffset);
                TrySpawnPowerupSafe(obstacleType, lane, segmentBase, laneOffset);
            }
        }

        private bool ShouldStartTrainRun()
        {
            float p = GameService.Instance.Difficulty.Progress;

            if (platformIndex < 5)
                return false;

            float chance = Mathf.Lerp(0.12f, 0.55f, p);

            if (GameService.Instance.Difficulty.CurrentSpeed > 20f)
                chance += 0.05f;

            chance = Mathf.Clamp01(chance);

            return Random.value < chance && trainSegmentsLeft == 0;
        }

        private void SpawnCoinsForSegment(
            ObstacleType type,
            int lane,
            Vector3 basePos,
            float laneOffset)
        {
            int coinCount = 6;
            int jumpCoinCount = 12;

            switch (type)
            {
                case ObstacleType.JumpOnly:
                    SpawnCoinArc(basePos, laneOffset, lane, jumpCoinCount);
                    break;

                case ObstacleType.SlideOnly:
                    SpawnCoinUnderSlide(basePos, laneOffset, lane, coinCount);
                    break;

                case ObstacleType.SlideOrJump:
                    {
                        bool spawnArc = Random.value > 0.5f;

                        if (spawnArc)
                        {
                            SpawnCoinArc(basePos, laneOffset, lane, jumpCoinCount);
                        }
                        else
                        {
                            SpawnStraightCoinRowOnLane(basePos, laneOffset, lane, coinCount);
                        }

                        break;
                    }

                case ObstacleType.Train:
                    return;

                default:
                    if (ShouldSpawnCoinsForEmptySegment())
                    {
                        SpawnRandomStraightCoinRow(basePos, laneOffset, coinCount);
                    }
                    break;
            }
        }

        private void TrySpawnPowerupSafe(ObstacleType obstacleType, int lane, Vector3 basePos, float laneOffset)
        {
            var powerupService = GameService.Instance.PowerupService;
            if (powerupService == null)
                return;

            float difficulty = GameService.Instance.Difficulty.Progress;

            segmentCounter++;

            if (segmentCounter < nextPowerupSegment)
                return;

            int selectedLane = lane;
            bool laneOk = !IsLaneBlocked(selectedLane, basePos, laneOffset) &&
                          !IsObstacleNearby(basePos, selectedLane, laneOffset);

            if (!laneOk)
            {
                for (int l = -1; l <= 1; l++)
                {
                    if (!IsLaneBlocked(l, basePos, laneOffset) &&
                        !IsObstacleNearby(basePos, l, laneOffset))
                    {
                        selectedLane = l;
                        laneOk = true;
                        break;
                    }
                }
            }

            if (!laneOk)
                return;

            float heightOffset = 0.55f;

            float finalHeight = platformTransform.position.y +
                                GameService.Instance.CoinService.BaseVerticalOffset +
                                heightOffset;

            Vector3 pos = new Vector3(
                basePos.x + selectedLane * laneOffset,
                finalHeight,
                basePos.z + 2.2f
            );

            PowerupType type = Random.value < 0.6f ? PowerupType.Magnet : PowerupType.Shield;
            powerupService.Spawn(type, pos);

            segmentCounter = 0;

            int minSeg = Mathf.RoundToInt(Mathf.Lerp(8f, 4f, difficulty));
            int maxSeg = Mathf.RoundToInt(Mathf.Lerp(14f, 6f, difficulty));
            nextPowerupSegment = Random.Range(minSeg, maxSeg + 1);
        }


        private bool IsLaneBlocked(int lane, Vector3 basePos, float laneOffset)
        {
            for (int i = 0; i < spawnedObstacles.Count; i++)
            {
                var obs = spawnedObstacles[i].ObstacleView;
                if (obs == null) continue;

                float dz = Mathf.Abs(obs.transform.position.z - basePos.z);
                if (dz > 4f) continue;

                float xDist = Mathf.Abs(obs.transform.position.x - (basePos.x + lane * laneOffset));
                if (xDist < laneOffset * 0.6f)
                    return true;
            }

            return false;
        }

        private bool IsObstacleNearby(Vector3 basePos, int lane, float laneOffset)
        {
            float safeRange = 6f;

            for (int i = 0; i < spawnedObstacles.Count; i++)
            {
                var obs = spawnedObstacles[i].ObstacleView;
                if (obs == null || !obs.gameObject.activeInHierarchy)
                    continue;

                float dz = Mathf.Abs(obs.transform.position.z - basePos.z);
                if (dz > safeRange)
                    continue;

                float xDistance = Mathf.Abs(obs.transform.position.x - (basePos.x + lane * laneOffset));

                if (xDistance < laneOffset * 0.6f)
                    return true;
            }

            return false;
        }

        private bool ShouldSpawnCoinsForEmptySegment()
        {
            float difficulty = GameService.Instance.Difficulty.Progress;
            float chance = Mathf.Lerp(0.8f, 0.2f, difficulty);
            return Random.value < chance;
        }

        private void SpawnRandomStraightCoinRow(
            Vector3 basePos,
            float laneOffset,
            int count)
        {
            int lane = GameService.Instance.ObstacleService.GetBalancedLane();
            SpawnStraightCoinRowOnLane(basePos, laneOffset, lane, count);
        }

        private void SpawnStraightCoinRowOnLane(
            Vector3 basePos,
            float laneOffset,
            int lane,
            int count)
        {
            float baseHeight =
                platformTransform.position.y +
                GameService.Instance.CoinService.BaseVerticalOffset;

            var coinService = GameService.Instance.CoinService;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    baseHeight,
                    basePos.z + (i * 1.5f)
                );

                CoinController coin = coinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(platformTransform);
                spawnedCoins.Add(coin);
            }
        }

        private void SpawnCoinArc(
            Vector3 basePos,
            float laneOffset,
            int lane,
            int count)
        {
            float obstacleHeight = 1.5f;
            Collider obstacleCollider = null;

            for (int i = 0; i < spawnedObstacles.Count; i++)
            {
                ObstacleView view = spawnedObstacles[i].ObstacleView;
                if (view == null) continue;

                if (Mathf.Abs(view.transform.position.z - basePos.z) < 0.5f)
                {
                    obstacleCollider = view.GetComponent<Collider>();
                    break;
                }
            }

            if (obstacleCollider != null)
                obstacleHeight = obstacleCollider.bounds.size.y;

            float maxArcHeight = Mathf.Clamp(obstacleHeight * 1.2f, 1.5f, 4f);

            float baseHeight = platformTransform.position.y +
                GameService.Instance.CoinService.BaseVerticalOffset;

            float arcStartOffset = -5f;
            float arcEndOffset = 5.5f;
            float step = (arcEndOffset - arcStartOffset) / (count - 1);

            var coinService = GameService.Instance.CoinService;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                float extraHeight = Mathf.Sin(t * Mathf.PI) * maxArcHeight;

                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    baseHeight + extraHeight,
                    basePos.z + arcStartOffset + (i * step)
                );

                CoinController coin = coinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(platformTransform);
                spawnedCoins.Add(coin);
            }
        }

        private void SpawnCoinUnderSlide(
            Vector3 basePos,
            float laneOffset,
            int lane,
            int count)
        {
            float height = platformTransform.position.y + 0.5f;
            var coinService = GameService.Instance.CoinService;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    height,
                    basePos.z + (i * 1.2f)
                );

                CoinController coin = coinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(platformTransform);
                spawnedCoins.Add(coin);
            }
        }

        public void ResetPlatform(Vector3 spawnPos, int index)
        {
            platformIndex = index;
            isDestroyed = false;
            hasSpawnedNext = false;
            trainSegmentsLeft = 0;

            var gameService = GameService.Instance;

            for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
            {
                gameService.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
            }
            spawnedObstacles.Clear();

            if (platformCollider == null)
                platformCollider = PlatformView.GetComponent<Collider>();

            platformCollider.enabled = false;

            platformTransform.position = spawnPos;
            PlatformView.gameObject.SetActive(true);

            platformCollider.enabled = true;

            SpawnObstaclesAndCoinsProcedural();
        }

        public void HandleCollision(Collider collider)
        {
            var gameService = GameService.Instance;

            if (collider.gameObject.CompareTag("Create") && !hasSpawnedNext)
            {
                hasSpawnedNext = true;

                float length = platformData.PlatformLength;

                Vector3 spawnPos = new Vector3(
                    platformTransform.position.x,
                    platformTransform.position.y,
                    platformTransform.position.z + length - 0.2f
                );

                PlatformController controller =
                    gameService.PlatformService.CreatePlatform(spawnPos);
            }

            if (collider.gameObject.CompareTag("Destroy"))
            {
                isDestroyed = true;

                for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
                {
                    gameService.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
                }
                spawnedObstacles.Clear();

                for (int i = spawnedCoins.Count - 1; i >= 0; i--)
                {
                    if (spawnedCoins[i].CoinView.transform.position.z <
                        gameService.PlayerService.GetPlayerZ() - 2f)
                    {
                        gameService.CoinService.ReturnCoinToPool(spawnedCoins[i]);
                        spawnedCoins.RemoveAt(i);
                    }
                }
                spawnedCoins.Clear();

                gameService.PlatformService.ReturnPlatformToPool(this);
                PlatformView.gameObject.SetActive(false);
            }
        }
    }
}
