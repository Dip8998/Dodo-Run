using TMPro;
using UnityEngine;
using DodoRun.Main;
using DodoRun.Tutorial;
using DodoRun.PowerUps;

namespace DodoRun.UI
{
    public sealed class TutorialUIController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private float visibleDuration = 3.5f;

        private TutorialService tutorial;
        private TutorialState lastState;
        private float timer;
        private bool visible;

        private void Start()
        {
            tutorial = GameService.Instance.TutorialService;
            panel.SetActive(false);
        }

        private void Update()
        {
            if (tutorial == null || !tutorial.IsActive)
            {
                panel.SetActive(false);
                return;
            }

            if (tutorial.CurrentState != lastState)
            {
                lastState = tutorial.CurrentState;
                Show();
            }

            if (!visible) return;

            timer += Time.deltaTime;
            if (timer >= visibleDuration)
            {
                panel.SetActive(false);
                visible = false;
            }
        }

        private void Show()
        {
            timer = 0f;
            visible = true;
            panel.SetActive(true);
            instructionText.text = GetText(lastState);
        }

        private string GetText(TutorialState state)
        {
            if (state == TutorialState.MagnetIntro)
                return GetPowerupText();

            string[] options = state switch
            {
                TutorialState.Welcome => new[]
                {
                    "Welcome to Dodo Run\nGet Ready!",
                    "Welcome!\nYour Run Starts Now",
                    "Welcome to Dodo Run\nSwipe to Survive"
                },

                TutorialState.TrainSwipe => new[]
                {
                    "Train Ahead!\nSwipe Left or Right",
                    "Avoid the Train\nSwipe Left or Right",
                    "Move Fast!\nSwipe Left or Right"
                },

                TutorialState.JumpOnly => new[]
                {
                    "Jump Over the Obstacle\nSwipe Up",
                    "Jump!\nSwipe Up to Avoid"
                },

                TutorialState.SlideOnly => new[]
                {
                    "Slide Under the Obstacle\nSwipe Down",
                    "Slide!\nSwipe Down to Avoid"
                },


                TutorialState.CoinTrail => new[]
                {
                    "Collect Coins\nIncrease Your Score",
                    "Grab Coins\nScore Goes Up",
                    "Coins Ahead\nCollect Them All"
                },

                _ => new[] { string.Empty }
            };

            return options[Random.Range(0, options.Length)];
        }

        private string GetPowerupText()
        {
            return tutorial.ActiveTutorialPowerup switch
            {
                PowerupType.Magnet =>
                    "Magnet Power!\nCoins Fly to You",

                PowerupType.Shield =>
                    "Shield Active!\nYou’re Safe from Obstacles",

                PowerupType.DoubleScore =>
                    "Double Score!\nEarn Points Faster",

                _ => string.Empty
            };
        }
    }
}
