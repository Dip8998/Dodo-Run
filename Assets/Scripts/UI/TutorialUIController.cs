using TMPro;
using UnityEngine;
using DodoRun.Tutorial;
using DodoRun.PowerUps;
using DodoRun.Main;

namespace DodoRun.UI
{
    public class TutorialUIController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private float visibleDuration = 3.5f;

        private TutorialService tutorial;
        private TutorialState lastState;
        private float timer;
        private bool isVisible;

        private void Start()
        {
            tutorial = GameService.Instance.TutorialService;
            panel.SetActive(false);
            isVisible = false;
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
                ShowPanel();
            }

            if (!isVisible) return;

            timer += Time.deltaTime;

            if (timer >= visibleDuration)
            {
                panel.SetActive(false);
                isVisible = false;
            }
        }

        private void ShowPanel()
        {
            timer = 0f;
            isVisible = true;
            panel.SetActive(true);
            instructionText.text = GetTextForState(lastState);
        }

        private string GetTextForState(TutorialState state)
        {
            if (state == TutorialState.MagnetIntro)
                return GetPowerupInstruction();

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

                TutorialState.JumpOrSlide => new[]
                {
            "Obstacle Ahead\nSwipe Up to Jump\nSwipe Down to Slide",
            "Jump or Slide\nSwipe Up or Down",
        },

                TutorialState.CoinTrail => new[]
                {
            "Collect Coins\nIncrease Your Score",
            "Grab Coins\nScore Goes Up",
            "Coins Ahead\nCollect Them All"
        },

                _ => new[] { "" }
            };

            return options[Random.Range(0, options.Length)];
        }

        private string GetPowerupInstruction()
        {
            if (tutorial == null) return "";

            return tutorial.ActiveTutorialPowerup switch
            {
                PowerupType.Magnet =>
                    "Magnet Power!\nCoins Fly to You",

                PowerupType.Shield =>
                    "Shield Active!\nSwipe Down to Slide Safely",

                PowerupType.DoubleScore =>
                    "Double Score!\nYour Score Increases Faster",

                _ => ""
            };
        }

    }
}
