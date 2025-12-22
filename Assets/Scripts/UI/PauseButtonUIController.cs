using UnityEngine;
using UnityEngine.UI;
using DodoRun.Main;
using DodoRun.Sound;

namespace DodoRun.UI
{
    public class PauseButtonUIController : MonoBehaviour
    {
        [SerializeField] private PauseUIController pauseUI;
        [SerializeField] private Button pauseButton;

        private void Start()
        {
            pauseButton.interactable = false;

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }
        }

        private void Update()
        {
            if (!pauseButton.interactable && GameService.Instance != null && GameService.Instance.IsInitialized)
            {
                pauseButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseClicked);
            }
        }

        public void OnPauseClicked()
        {
            if (GameService.Instance == null || !GameService.Instance.IsInitialized)
            {
                Debug.LogWarning("Pause clicked but Game is not running yet (Loading Assets...)");
                return;
            }

            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            AudioManager.Instance.SetRunningSoundActive(false);
            pauseUI.Pause();
        }
    }
}