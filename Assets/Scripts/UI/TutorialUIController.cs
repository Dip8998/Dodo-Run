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
        private TutorialState lastState = TutorialState.None;
        private float timer;
        private bool visible;
        private PowerupType lastPowerup;

        private void Start()
        {
            tutorial = GameService.Instance.TutorialService;
            if (panel != null) panel.SetActive(false);
        }

        private void Update()
        {
            if (tutorial == null)
            {
                tutorial = GameService.Instance.TutorialService;
                return;
            }

            if (!tutorial.IsActive)
            {
                if (panel.activeSelf) panel.SetActive(false);
                return;
            }

            if (tutorial.CurrentState != lastState)
            {
                lastState = tutorial.CurrentState;
                Show();
            }
            else if (tutorial.CurrentState == TutorialState.MagnetIntro &&
                     tutorial.ActiveTutorialPowerup != lastPowerup)
            {
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
            if (lastState == TutorialState.None || lastState == TutorialState.Finished) return;

            timer = 0f;
            visible = true;
            panel.SetActive(true);

            lastPowerup = tutorial.ActiveTutorialPowerup;
            instructionText.text = GetText(lastState);
        }

        private string GetText(TutorialState state)
        {
            if (state == TutorialState.MagnetIntro)
                return GetPowerupText();

            string[] options = state switch
            {
                TutorialState.Welcome => new[] { "Welcome to Dodo Run\nGet Ready!", "Your Run Starts Now" },
                TutorialState.TrainSwipe => new[] { "Train Ahead!\nSwipe Left or Right" },
                TutorialState.JumpOnly => new[] { "Jump Over!\nSwipe Up" },
                TutorialState.SlideOnly => new[] { "Slide Under!\nSwipe Down" },
                TutorialState.CoinTrail => new[] { "Collect Coins! and Increase your score" },
                _ => new[] { string.Empty }
            };

            return options[Random.Range(0, options.Length)];
        }

        private string GetPowerupText()
        {
            return lastPowerup switch
            {
                PowerupType.Magnet => "Magnet Power!\nCoins Fly to You",
                PowerupType.Shield => "Shield Active!\nYou’re Safe",
                PowerupType.DoubleScore => "Double Score!\nEarn Faster",
                _ => "Powerup Collected!"
            };
        }
    }
}