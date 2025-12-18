using DodoRun.Main;
using DodoRun.Obstacle;
using DodoRun.Coin;
using DodoRun.PowerUps;
using DodoRun.Platform;
using UnityEngine;

namespace DodoRun.Tutorial
{
    public class TutorialSpawner
    {
        private readonly GameService game;

        public TutorialSpawner(GameService game)
        {
            this.game = game;
        }

        private PlatformController GetActivePlatform()
        {
            return game.PlatformService.GetLatestPlatform();
        }

        public void SpawnTrain(Vector3 basePos)
        {
            PlatformController platform = GetActivePlatform();
            if (platform == null) return;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;

            Vector3 pos = basePos;
            pos.y = platform.PlatformView.transform.position.y + 1.3f;

            platform.SpawnTutorialObstacle(
                ObstacleType.Train,
                0,
                pos,
                laneOffset
            );
        }

        public void SpawnJumpOrSlide(Vector3 basePos)
        {
            PlatformController platform = GetActivePlatform();
            if (platform == null) return;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;
            float platformY = platform.PlatformView.transform.position.y + 0.25f;

            basePos.x = platform.PlatformView.transform.position.x;

            int[] lanes = { -1, 0, 1 };

            for (int i = 0; i < lanes.Length; i++)
            {
                Vector3 pos = basePos;
                pos.y = platformY;

                platform.SpawnTutorialObstacle(
                    ObstacleType.SlideOrJump,
                    lanes[i],
                    pos,
                    laneOffset
                );
            }
        }

        public void SpawnCoinTrail(Vector3 startPos, int count = 10)
        {
            PlatformController platform = GetActivePlatform();
            if (platform == null) return;

            float baseY =
                platform.PlatformView.transform.position.y +
                game.CoinService.BaseVerticalOffset;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = startPos + Vector3.forward * (i * 1.5f);
                pos.y = baseY;
                platform.SpawnTutorialCoin(pos);
            }
        }

        public PowerupType SpawnRandomPowerupTutorial(Vector3 basePos)
        {
            PlatformController platform = GetActivePlatform();
            if (platform == null) return PowerupType.None;

            basePos.x = platform.PlatformView.transform.position.x;

            float laneOffset = game.PlatformService.PlatformScriptableObject.LaneOffset;
            float platformY = platform.PlatformView.transform.position.y;
            float coinBaseY = platformY + game.CoinService.BaseVerticalOffset;
            float obstacleY = platformY + 0.25f;

            float r = Random.value;

            if (r < 0.33f)
            {
                Vector3 magnetPos = basePos;
                magnetPos.y = coinBaseY + 0.6f;

                game.PowerupService.Spawn(PowerupType.Magnet, magnetPos);

                float zGap = 10f;

                SpawnCoinTrail(magnetPos + new Vector3(-laneOffset, 0f, zGap));
                SpawnCoinTrail(magnetPos + new Vector3(0f, 0f, zGap));
                SpawnCoinTrail(magnetPos + new Vector3(laneOffset, 0f, zGap));

                SpawnCoinTrail(magnetPos + new Vector3(-laneOffset, 0f, zGap + 20f));
                SpawnCoinTrail(magnetPos + new Vector3(0f, 0f, zGap + 20f));
                SpawnCoinTrail(magnetPos + new Vector3(laneOffset, 0f, zGap + 20f));

                SpawnCoinTrail(magnetPos + new Vector3(-laneOffset, 0f, zGap + 40f));
                SpawnCoinTrail(magnetPos + new Vector3(0f, 0f, zGap + 40f));
                SpawnCoinTrail(magnetPos + new Vector3(laneOffset, 0f, zGap + 40f));

                return PowerupType.Magnet;
            }
            else if (r < 0.66f)
            {
                Vector3 shieldPos = basePos;
                shieldPos.y = coinBaseY + 0.6f;

                game.PowerupService.Spawn(PowerupType.Shield, shieldPos);

                int[] lanes = { -1, 0, 1 };

                float obstacleZGap = 12f;

                for (int i = 0; i < lanes.Length; i++)
                {
                    Vector3 pos = basePos;
                    pos.y = obstacleY;
                    pos.z += obstacleZGap;

                    platform.SpawnTutorialObstacle(
                        ObstacleType.SlideOnly,
                        lanes[i],
                        pos,
                        laneOffset
                    );
                }

                return PowerupType.Shield;
            }
            else
            {
                Vector3 doubleScorePos = basePos;
                doubleScorePos.y = coinBaseY + 0.6f;

                game.PowerupService.Spawn(PowerupType.DoubleScore, doubleScorePos);
                return PowerupType.DoubleScore;
            }
        }
    }
}
