using UnityEngine;
using UnityEngine.UI;
using DodoRun.Main;

namespace DodoRun.UI
{
    public class PauseButtonUIController : MonoBehaviour
    {
        [SerializeField] private PauseUIController pauseUI;
        [SerializeField] private Button pauseButton;

        private void Start()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
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
            if (GameService.Instance != null && GameService.Instance.IsGameRunning)
            {
                pauseUI.Pause();
            }
            else
            {
                Debug.LogWarning("Pause clicked but Game is not running yet (Loading Assets...)");
            }
        }
    }
}