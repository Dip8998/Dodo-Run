using DodoRun.Main;
using DodoRun.Player;
using DodoRun.PowerUps;
using UnityEngine;

namespace DodoRun.Tutorial
{
    public class TutorialService
    {
        public bool IsActive { get; private set; } = true;
        public TutorialState CurrentState { get; private set; }
        public PowerupType ActiveTutorialPowerup { get; private set; }

        private GameService game;
        private TutorialInputGate inputGate;
        private TutorialSpawner spawner;
        private float timer;
        private bool stepSpawned;

        private PlayerController player;

        public void StartTutorial()
        {
            game = GameService.Instance;

            player = game.PlayerService.GetPlayerController();

            inputGate = new TutorialInputGate(player);
            spawner = new TutorialSpawner(game);

            CurrentState = TutorialState.Welcome;
            timer = 0f;
        }

        public void UpdateTutorial()
        {
            if (!IsActive) return;

            timer += Time.deltaTime;

            switch (CurrentState)
            {
                case TutorialState.Welcome:
                    HandleWelcome();
                    break;

                case TutorialState.TrainSwipe:
                    HandleTrain();
                    break;

                case TutorialState.JumpOrSlide:
                    HandleJumpSlide();
                    break;

                case TutorialState.CoinTrail:
                    HandleCoin();
                    break;

                case TutorialState.MagnetIntro:
                    HandleMagnet();
                    break;
            }
        }

        private void HandleWelcome()
        {
            inputGate.LockAll();

            if (timer >= 7f)
            {
                stepSpawned = false;
                timer = 0f;
                CurrentState = TutorialState.TrainSwipe;

                inputGate.EnableSwipeOnly();
            }
        }

        private void HandleTrain()
        {
            if (!stepSpawned)
            {
                spawner.SpawnTrain(player.PlayerView.transform.position + Vector3.forward * 28f);
                stepSpawned = true;
            }

            if (timer >= 7f)
            {
                stepSpawned = false;
                timer = 0f;
                CurrentState = TutorialState.JumpOrSlide;
                inputGate.EnableAll();
            }
        }

        private void HandleJumpSlide()
        {
            if (!stepSpawned)
            {
                spawner.SpawnJumpOrSlide(player.PlayerView.transform.position + Vector3.forward * 28f);
                stepSpawned = true;
            }

            if (timer >= 7f)
            {
                stepSpawned = false;
                timer = 0f;
                CurrentState = TutorialState.CoinTrail;
            }
        }

        private void HandleCoin()
        {
            if (!stepSpawned)
            {
                spawner.SpawnCoinTrail(player.PlayerView.transform.position + Vector3.forward * 25f);
                stepSpawned = true;
            }

            if (timer >= 7f)
            {
                stepSpawned = false;
                timer = 0f;
                CurrentState = TutorialState.MagnetIntro;
            }
        }

        private void HandleMagnet()
        {
            if (!stepSpawned)
            {
                ActiveTutorialPowerup = spawner.SpawnRandomPowerupTutorial(player.PlayerView.transform.position + Vector3.forward * 28f);
                stepSpawned = true;
            }

            if (timer >= 7f)
            {
                EndTutorial();
            }
        }

        private void EndTutorial()
        {
            IsActive = false;
            CurrentState = TutorialState.Finished;
        }

        public bool CanProcessSwipe(Vector2 direction)
        {
            if (!IsActive) return true;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return inputGate.AllowLeftRight;

            if (direction.y > 0)
                return inputGate.AllowJump;

            return inputGate.AllowSlide;
        }
    }
}
