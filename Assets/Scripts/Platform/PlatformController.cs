using DodoRun.Coin;
using DodoRun.Main;
using DodoRun.Obstacle;
using System.Collections.Generic;
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
        private List<int> segmentContextTags = new List<int>();
        private List<ObstacleController> spawnedObstacles = new List<ObstacleController>();
        private List<CoinController> spawnedCoins = new List<CoinController>();

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
            SpawnObstacle(platformIndex);
            SpawnCoins();
        }

        public void UpdatePlatform()
        {
            if (isDestroyed || rigidbody == null) return;

            if (!GameService.Instance.IsGameRunning)
            {
                return;
            }

            Vector3 movement = new Vector3(0, 0, -platformData.MoveSpeed * Time.deltaTime);
            PlatformView.transform.position += movement;

            foreach (ObstacleController obstacle in spawnedObstacles)
            {
                if (obstacle.ObstacleView != null && obstacle.ObstacleView.gameObject.activeInHierarchy)
                {
                    obstacle.ObstacleView.transform.position += movement;
                }
            }
        }

        private void SpawnObstacle(int index)
        {
            segmentContextTags.Clear();
            if (index == 1)
            {
                return;
            }

            ObstacleScriptableObject obstacleScriptableObject =
                GameService.Instance.ObstacleService.ObstacleScriptableObject;

            float platformLength = platformData.PlatformLength;
            float segmentLength = obstacleScriptableObject.ObstacleSegmentLength;
            float spawnProbability = obstacleScriptableObject.SpawnProbability;
            float laneOffset = platformData.LaneOffset;

            int numberOfSegments = Mathf.FloorToInt(platformLength / segmentLength);

            float platformBackEdgeZ = PlatformView.transform.position.z - (platformLength / 2f);

            int startSegmentIndex = Mathf.CeilToInt(platformData.SafeZoneDistance / segmentLength);

            for (int i = startSegmentIndex; i < numberOfSegments; i++)
            {
                bool shouldSpawn = (i == startSegmentIndex) || (Random.value < spawnProbability);
                segmentContextTags.Add(0);

                if (shouldSpawn)
                {
                    float segmentZ = platformBackEdgeZ
                                            + (i * segmentLength)
                                            + (segmentLength / 2f);

                    Vector3 segmentSpawnBase = new Vector3(
                        PlatformView.transform.position.x,
                        PlatformView.transform.position.y,
                        segmentZ
                    );

                    int obstacleTag = 0;
                    List<ObstacleController> patternControllers =
                        GameService.Instance.ObstacleService.SpawnRandomPattern(
                            segmentSpawnBase,
                            laneOffset,
                            PlatformView.transform,
                            out obstacleTag 
                        );

                    if (patternControllers != null && obstacleTag != 0)
                    {
                        spawnedObstacles.AddRange(patternControllers);

                        int currentSegmentIndex = segmentContextTags.Count - 1;
                        segmentContextTags[currentSegmentIndex] = obstacleTag;
                    }
                }
            }
        }

        private void SpawnCoins()
        {
            CoinScriptableObject coinScriptableObject = GameService.Instance.CoinService.CoinScriptableObject;

            float platformLength = platformData.PlatformLength;
            float segmentLength = GameService.Instance.ObstacleService.ObstacleScriptableObject.ObstacleSegmentLength;
            float laneOffset = platformData.LaneOffset;

            int numberOfSegments = Mathf.FloorToInt(platformLength / segmentLength);
            int startSegmentIndex = Mathf.CeilToInt(platformData.SafeZoneDistance / segmentLength);

            float platformBackEdgeZ = PlatformView.transform.position.z - (platformLength / 2f);

            for (int i = startSegmentIndex; i < numberOfSegments; i++)
            {
                int segmentListIndex = i - startSegmentIndex;

                if (segmentListIndex >= segmentContextTags.Count) continue;

                int obstacleContextTag = segmentContextTags[segmentListIndex];

                float segmentZ = platformBackEdgeZ
                                 + (i * segmentLength)
                                 + (segmentLength / 2f);

                Vector3 segmentSpawnBase = new Vector3(
                    PlatformView.transform.position.x,
                    PlatformView.transform.position.y,
                    segmentZ
                );

                List<CoinController> patternControllers = null;

                if (obstacleContextTag != 0)
                {
                    patternControllers =
                        GameService.Instance.CoinService.SpawnContextualCoinPattern(
                            segmentSpawnBase,
                            laneOffset,
                            PlatformView.transform,
                            obstacleContextTag 
                        );
                }
                else if (Random.value < coinScriptableObject.SpawnProbability)
                {
                    patternControllers =
                        GameService.Instance.CoinService.SpawnRandomCoinPattern(
                            segmentSpawnBase,
                            laneOffset,
                            PlatformView.transform
                        );
                }

                if (patternControllers != null)
                {
                    spawnedCoins.AddRange(patternControllers);
                }
            }
        }

        public void ResetPlatform(Vector3 spawnPos, int index)
        {
            platformIndex = index;

            isDestroyed = false;
            hasSpawnedNext = false;

            for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
            {
                GameService.Instance.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
            }
            spawnedObstacles.Clear();

            for (int i = spawnedCoins.Count - 1; i >= 0; i--)
            {
                GameService.Instance.CoinService.ReturnCoinToPool(spawnedCoins[i]);
            }
            spawnedCoins.Clear();

            Collider col = PlatformView.GetComponent<Collider>();
            col.enabled = false;

            PlatformView.transform.position = spawnPos;

            PlatformView.gameObject.SetActive(true);

            col.enabled = true;

            SpawnObstacle(platformIndex);
            SpawnCoins();
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

                PlatformController controller = GameService.Instance.PlatformService.CreatePlatform(spawnPos);
            }

            if (collider.gameObject.CompareTag("Destroy"))
            {
                isDestroyed = true;

                for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
                {
                    GameService.Instance.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
                }
                spawnedObstacles.Clear();

                GameService.Instance.PlatformService.ReturnPlatformToPool(this);
                PlatformView.gameObject.SetActive(false);
                return;
            }
        }
    }
}