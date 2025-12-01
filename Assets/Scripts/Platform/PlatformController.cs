using System.Collections.Generic;
using DodoRun.Coin;
using DodoRun.Main;
using DodoRun.Obstacle;
using UnityEngine;

namespace DodoRun.Platform
{
    public class PlatformController
    {
        private PlatformScriptableObject platformData;
        public PlatformView PlatformView { get; private set; }

        private Rigidbody rigidbody;
        private bool hasSpawnedNext = false;
        private bool isDestroyed = false;
        private int platformIndex;

        private readonly List<ObstacleController> spawnedObstacles = new List<ObstacleController>();
        private readonly List<CoinController> spawnedCoins = new List<CoinController>();

        private const float SegmentLength = 10f;
        private int trainSegmentsLeft = 0;
        private int trainLane = 0;

        public PlatformController(PlatformScriptableObject platformScriptableObject, Vector3 spawnPos, int index)
        {
            platformData = platformScriptableObject;
            platformIndex = index;
            SetupView(spawnPos);
        }

        private void SetupView(Vector3 spawnPos)
        {
            PlatformView = Object.Instantiate(platformData.Platform, spawnPos, Quaternion.identity);
            rigidbody = PlatformView.GetComponent<Rigidbody>();
            PlatformView.SetController(this);

            SpawnObstaclesAndCoinsProcedural();
        }

        public void UpdatePlatform()
        {
            if (isDestroyed || rigidbody == null) return;
            if (!GameService.Instance.IsGameRunning) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;
            Vector3 movement = new Vector3(0f, 0f, -speed * Time.deltaTime);

            PlatformView.transform.position += movement;

            foreach (ObstacleController obstacle in spawnedObstacles)
            {
                if (obstacle.ObstacleView != null && obstacle.ObstacleView.gameObject.activeInHierarchy)
                {
                    obstacle.ObstacleView.transform.position += movement;
                }
            }
        }

        private void SpawnObstaclesAndCoinsProcedural()
        {
            spawnedObstacles.Clear();
            spawnedCoins.Clear();

            float platformLength = platformData.PlatformLength;
            float laneOffset = platformData.LaneOffset;

            float platformBackEdgeZ = PlatformView.transform.position.z - (platformLength / 2f);
            int numberOfSegments = Mathf.FloorToInt(platformLength / SegmentLength);
            int startSegmentIndex = Mathf.CeilToInt((platformData.SafeZoneDistance + 10f) / SegmentLength);
    
            for (int i = startSegmentIndex; i < numberOfSegments; i++)
            {
                float segmentZ =
                    platformBackEdgeZ +
                    (i * SegmentLength) +
                    (SegmentLength / 2f);

                Vector3 segmentBase = new Vector3(
                    PlatformView.transform.position.x,
                    PlatformView.transform.position.y,
                    segmentZ
                );

                ObstacleType obstacleType = GameService.Instance.ObstacleService.GetBalancedRandomObstacleType();
                int lane = GameService.Instance.ObstacleService.GetBalancedLane();

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
                        float p = GameService.Instance.Difficulty.Progress;

                        obstacleType = ObstacleType.Train;
                        lane = GameService.Instance.ObstacleService.GetBalancedLane();
                        trainLane = lane;

                        int minTrainSize = Mathf.RoundToInt(Mathf.Lerp(2f, 4f, p));
                        int maxTrainSize = Mathf.RoundToInt(Mathf.Lerp(4f, 9f, p));

                        trainSegmentsLeft = Random.Range(minTrainSize, maxTrainSize);
                    }
                    else
                    {
                        obstacleType = GameService.Instance.ObstacleService.GetBalancedRandomObstacleType();
                        lane = GameService.Instance.ObstacleService.GetBalancedLane();
                    }
                }

                Vector3 spawnPos = segmentBase;

                if (obstacleType == ObstacleType.Train)
                {
                    spawnPos.y = PlatformView.transform.position.y + 1.3f; 
                }
                else
                {
                    spawnPos.y = PlatformView.transform.position.y;
                }

                ObstacleController obstacle =
                    GameService.Instance.ObstacleService.SpawnObstacle(
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
                PlatformView.transform.position.y +
                GameService.Instance.CoinService.BaseVerticalOffset;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    baseHeight,
                    basePos.z + (i * 1.5f)
                );

                CoinController coin = GameService.Instance.CoinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(PlatformView.transform);
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

            foreach (var obs in spawnedObstacles)
            {
                if (Mathf.Abs(obs.ObstacleView.transform.position.z - basePos.z) < 0.5f)
                {
                    obstacleCollider = obs.ObstacleView.GetComponent<Collider>();
                    break;
                }
            }

            if (obstacleCollider != null)
                obstacleHeight = obstacleCollider.bounds.size.y;

            float maxArcHeight = Mathf.Clamp(obstacleHeight * 1.2f, 1.5f, 4f);

            float baseHeight = PlatformView.transform.position.y + GameService.Instance.CoinService.BaseVerticalOffset;

            float arcStartOffset = -5f;
            float arcEndOffset = 5.5f;
            float step = (arcEndOffset - arcStartOffset) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                float extraHeight = Mathf.Sin(t * Mathf.PI) * maxArcHeight;

                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    baseHeight + extraHeight,
                    basePos.z + arcStartOffset + (i * step)
                );

                CoinController coin = GameService.Instance.CoinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(PlatformView.transform);
                spawnedCoins.Add(coin);
            }
        }

        private void SpawnCoinUnderSlide(
            Vector3 basePos,
            float laneOffset,
            int lane,
            int count)
        {
            float height = PlatformView.transform.position.y + 0.5f;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    basePos.x + lane * laneOffset,
                    height,
                    basePos.z + (i * 1.2f)
                );

                CoinController coin = GameService.Instance.CoinService.GetCoin(pos);
                coin.CoinView.transform.SetParent(PlatformView.transform);
                spawnedCoins.Add(coin);
            }
        }

        public void ResetPlatform(Vector3 spawnPos, int index)
        {
            platformIndex = index;
            isDestroyed = false;
            hasSpawnedNext = false;
            trainSegmentsLeft = 0;

            for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
            {
                GameService.Instance.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
            }
            spawnedObstacles.Clear();

            Collider col = PlatformView.GetComponent<Collider>();
            col.enabled = false;

            PlatformView.transform.position = spawnPos;
            PlatformView.gameObject.SetActive(true);

            col.enabled = true;

            SpawnObstaclesAndCoinsProcedural();
        }

        public void HandleCollision(Collider collider)
        {
            if (collider.gameObject.CompareTag("Create") && !hasSpawnedNext)
            {
                hasSpawnedNext = true;

                float length = platformData.PlatformLength;

                Vector3 spawnPos = new Vector3(
                    PlatformView.transform.position.x,
                    PlatformView.transform.position.y,
                    PlatformView.transform.position.z + length - 0.2f
                );

                PlatformController controller =
                    GameService.Instance.PlatformService.CreatePlatform(spawnPos);
            }

            if (collider.gameObject.CompareTag("Destroy"))
            {
                isDestroyed = true;

                for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
                {
                    GameService.Instance.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
                }
                spawnedObstacles.Clear();

                for (int i = spawnedCoins.Count - 1; i >= 0; i--)
                {
                    if (spawnedCoins[i].CoinView.transform.position.z < GameService.Instance.PlayerService.GetPlayerZ() - 2f)
                    {
                        GameService.Instance.CoinService.ReturnCoinToPool(spawnedCoins[i]);
                        spawnedCoins.RemoveAt(i);
                    }
                }
                spawnedCoins.Clear();

                GameService.Instance.PlatformService.ReturnPlatformToPool(this);
                PlatformView.gameObject.SetActive(false);
            }
        }
    }
}