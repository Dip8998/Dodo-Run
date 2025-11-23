using DodoRun.Main;
using DodoRun.Obstacle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DodoRun.Platform
{
    public class PlatformController
    {
        private PlatformScriptableObject platformData;
        public PlatformView platformView {get; private set;}
        private Rigidbody rigidbody;
        private bool hasSpawnedNext = false;
        private bool isDestroyed = false;
        private List<ObstacleController> spawnedObstacles = new List<ObstacleController>();

        public PlatformController(PlatformScriptableObject platformScriptableObject, Vector3 spawnPos)
        {
            platformData = platformScriptableObject;
            SetupView(spawnPos);
        }

        private void SetupView(Vector3 spawnPos)
        {
            platformView = Object.Instantiate(platformData.Platform, spawnPos, Quaternion.identity);
            rigidbody = platformView.GetComponent<Rigidbody>();
            platformView.SetController(this);
            SpawnObstacle();
        }

        public void UpdatePlatform()
        {
            if (isDestroyed || rigidbody == null) return;
            platformView.transform.position += new Vector3(0, 0, -platformData.MoveSpeed * Time.deltaTime);
        }

        private void SpawnObstacle()
        {
            ObstacleScriptableObject obstacleScriptableObject =
                GameService.Instance.ObstacleService.ObstacleScriptableObject;

            float platformLength = platformData.PlatformLength;
            float segmentLength = obstacleScriptableObject.ObstacleSegmentLength;
            float spawnProbability = obstacleScriptableObject.SpawnProbability;
            float laneOffset = platformData.LaneOffset;

            int numberOfSegments = Mathf.FloorToInt(platformLength / segmentLength);

            for (int i = 0; i < numberOfSegments; i++)
            {
                if (Random.value < spawnProbability)
                {
                    float segmentZ = platformView.transform.position.z
                                         + (i * segmentLength)
                                         + (segmentLength / 2f);

                    Vector3 segmentSpawnBase = new Vector3(
                        platformView.transform.position.x,
                        platformView.transform.position.y,
                        segmentZ
                    );

                    List<ObstacleController> patternControllers =
                        GameService.Instance.ObstacleService.SpawnRandomPattern(
                            segmentSpawnBase,
                            laneOffset,
                            platformView.transform
                        );

                    if (patternControllers != null)
                    {
                        spawnedObstacles.AddRange(patternControllers);
                    }
                }
            }
        }

        public void ResetPlatform(Vector3 spawnPos)
        {
            isDestroyed = false;
            hasSpawnedNext = false;

            for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
            {
                GameService.Instance.ObstacleService.ReturnObstacleToPool(spawnedObstacles[i]);
            }
            spawnedObstacles.Clear(); 

            Collider col = platformView.GetComponent<Collider>();
            col.enabled = false;

            platformView.transform.position = spawnPos;

            platformView.gameObject.SetActive(true);

            col.enabled = true;

            SpawnObstacle();
        }

        public void HandleCollision(Collider collider)
        {
            if (collider.gameObject.CompareTag("Create") && !hasSpawnedNext)
            {
                hasSpawnedNext = true;

                float length = platformData.PlatformLength;

                Vector3 spawnPos = new Vector3(
                    platformView.transform.position.x,
                    platformView.transform.position.y,
                    platformView.transform.position.z + length - 0.2f
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
                platformView.gameObject.SetActive(false);
                return;
            }
        }
    }
}
