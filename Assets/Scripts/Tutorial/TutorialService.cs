using UnityEngine;
using DodoRun.Main;
using DodoRun.Player;
using DodoRun.PowerUps;
using DodoRun.Data;

namespace DodoRun.Tutorial
{
    public sealed class TutorialService
    {
        public bool IsActive { get; private set; } = true;
        public TutorialState CurrentState { get; private set; }
        public PowerupType ActiveTutorialPowerup { get; private set; }

        private GameService game;
        private PlayerController player;
        private TutorialInputGate inputGate;
        private TutorialSpawner spawner;

        private float timer;
        private bool hasSpawnedForState;

        public void StartTutorial()
        {
            if (PlayerDataService.IsTutorialCompleted)
            {
                IsActive = false;
                CurrentState = TutorialState.Finished;
                return;
            }

            game = GameService.Instance;
            player = game.PlayerService.GetPlayerController();

            inputGate = new TutorialInputGate(player);
            spawner = new TutorialSpawner(game);

            EnterState(TutorialState.Welcome);
        }

        public void UpdateTutorial()
        {
            if (!IsActive)
                return;

            timer += Time.deltaTime;

            switch (CurrentState)
            {
                case TutorialState.Welcome:
                    if (timer >= 7f)
                        EnterState(TutorialState.TrainSwipe);
                    break;

                case TutorialState.TrainSwipe:
                    SpawnOnce(() =>
                        spawner.SpawnTrain(SpawnAhead(28f))
                    );

                    if (timer >= 7f)
                        EnterState(TutorialState.JumpOnly);
                    break;

                case TutorialState.JumpOnly:
                    SpawnOnce(() =>
                        spawner.SpawnJumpOnly(SpawnAhead(28f))
                    );

                    if (timer >= 7f)
                        EnterState(TutorialState.SlideOnly);
                    break;

                case TutorialState.SlideOnly:
                    SpawnOnce(() =>
                        spawner.SpawnSlideOnly(SpawnAhead(28f))
                    );

                    if (timer >= 7f)
                        EnterState(TutorialState.CoinTrail);
                    break;

                case TutorialState.CoinTrail:
                    SpawnOnce(() =>
                        spawner.SpawnCoinTrail(SpawnAhead(25f))
                    );

                    if (timer >= 7f)
                        EnterState(TutorialState.MagnetIntro);
                    break;

                case TutorialState.MagnetIntro:
                    SpawnOnce(() =>
                        ActiveTutorialPowerup =
                            spawner.SpawnRandomPowerupTutorial(SpawnAhead(28f))
                    );

                    if (timer >= 7f)
                        EndTutorial();
                    break;
            }
        }

        private void EnterState(TutorialState state)
        {
            CurrentState = state;
            timer = 0f;
            hasSpawnedForState = false;

            switch (state)
            {
                case TutorialState.Welcome:
                    inputGate.LockAll();
                    break;

                case TutorialState.TrainSwipe:
                    inputGate.EnableSwipeOnly();
                    break;

                case TutorialState.JumpOnly:
                    inputGate.EnableAll();
                    break;

                case TutorialState.SlideOnly:
                    inputGate.EnableAll();
                    break;
            }
        }

        private void SpawnOnce(System.Action action)
        {
            if (hasSpawnedForState)
                return;

            action.Invoke();
            hasSpawnedForState = true;
        }

        private Vector3 SpawnAhead(float zOffset)
        {
            return player.PlayerView.transform.position + Vector3.forward * zOffset;
        }

        private void EndTutorial()
        {
            IsActive = false;
            CurrentState = TutorialState.Finished;
            inputGate.EnableAll();

            PlayerDataService.MarkTutorialCompleted();
        }

        public bool CanProcessSwipe(Vector2 direction)
        {
            if (!IsActive)
                return true;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return inputGate.AllowLeftRight;

            return direction.y > 0
                ? inputGate.AllowJump
                : inputGate.AllowSlide;
        }
    }
}
