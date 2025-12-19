using UnityEngine;
using DodoRun.Main;
using DodoRun.Obstacle;
using DodoRun.PowerUps;
using DodoRun.Platform;

namespace DodoRun.Tutorial
{
    public sealed class TutorialSpawner
    {
        private readonly GameService game;

        public TutorialSpawner(GameService game)
        {
            this.game = game;
        }

        private PlatformController Platform =>
            game.PlatformService.GetLatestPlatform();

        public void SpawnTrain(Vector3 basePos)
        {
            var platform = Platform;
            if (platform == null) return;

            basePos.y = platform.PlatformView.transform.position.y + 1.3f;

            platform.SpawnTutorialObstacle(
                ObstacleType.Train,
                0,
                basePos,
                game.PlatformService.PlatformScriptableObject.LaneOffset
            );
        }

        public void SpawnJumpOnly(Vector3 basePos)
        {
            PlatformController platform = Platform;
            if (platform == null) return;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;
            float yPos = platform.PlatformView.transform.position.y + 0.25f;

            basePos.x = platform.PlatformView.transform.position.x;
            basePos.y = yPos;

            int[] lanes = { -1, 0, 1 };

            for (int i = 0; i < lanes.Length; i++)
            {
                platform.SpawnTutorialObstacle(
                    ObstacleType.JumpOnly,
                    lanes[i],
                    basePos,
                    laneOffset
                );
            }
        }

        public void SpawnSlideOnly(Vector3 basePos)
        {
            PlatformController platform = Platform;
            if (platform == null) return;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;
            float yPos = platform.PlatformView.transform.position.y + 0.25f;

            basePos.x = platform.PlatformView.transform.position.x;
            basePos.y = yPos;

            int[] lanes = { -1, 0, 1 };

            for (int i = 0; i < lanes.Length; i++)
            {
                platform.SpawnTutorialObstacle(
                    ObstacleType.SlideOnly,
                    lanes[i],
                    basePos,
                    laneOffset
                );
            }
        }

        public void SpawnCoinTrail(Vector3 startPos, int count = 10)
        {
            var platform = Platform;
            if (platform == null) return;

            float y =
                platform.PlatformView.transform.position.y +
                game.CoinService.BaseVerticalOffset;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = startPos + Vector3.forward * (i * 1.5f);
                pos.y = y;
                platform.SpawnTutorialCoin(pos);
            }
        }

        public PowerupType SpawnRandomPowerupTutorial(Vector3 basePos)
        {
            var platform = Platform;
            if (platform == null) return PowerupType.None;

            basePos.x = platform.PlatformView.transform.position.x;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;
            float platformY = platform.PlatformView.transform.position.y;
            float coinY = platformY + game.CoinService.BaseVerticalOffset + 0.6f;

            float r = Random.value;

            if (r < 0.33f)
            {
                game.PowerupService.Spawn(PowerupType.Magnet,
                    new Vector3(basePos.x, coinY, basePos.z));

                SpawnCoinGrid(basePos, laneOffset);
                return PowerupType.Magnet;
            }

            if (r < 0.66f)
            {
                game.PowerupService.Spawn(PowerupType.Shield,
                    new Vector3(basePos.x, coinY, basePos.z));

                SpawnSlideWall(basePos, laneOffset, platformY + 0.25f);
                return PowerupType.Shield;
            }

            game.PowerupService.Spawn(PowerupType.DoubleScore,
                new Vector3(basePos.x, coinY, basePos.z));

            return PowerupType.DoubleScore;
        }

        private void SpawnCoinGrid(Vector3 origin, float laneOffset)
        {
            float[] zOffsets = { 10f, 30f, 50f };

            for (int z = 0; z < zOffsets.Length; z++)
            {
                SpawnCoinTrail(origin + new Vector3(-laneOffset, 0f, zOffsets[z]));
                SpawnCoinTrail(origin + new Vector3(0f, 0f, zOffsets[z]));
                SpawnCoinTrail(origin + new Vector3(laneOffset, 0f, zOffsets[z]));
            }
        }

        public void SpawnSlideWall(Vector3 basePos, float laneOffset, float y)
        {
            int[] lanes = { -1, 0, 1 };
            float zGap = 12f;

            for (int i = 0; i < lanes.Length; i++)
            {
                Vector3 pos = basePos;
                pos.y = y;
                pos.z += zGap;

                Platform.SpawnTutorialObstacle(
                    ObstacleType.SlideOnly,
                    lanes[i],
                    pos,
                    laneOffset
                );
            }
        }
    }
}
