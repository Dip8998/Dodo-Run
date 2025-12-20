using UnityEngine;
using UnityEngine.UI;

namespace DodoRun.UI
{
    public class PauseButtonUIController : MonoBehaviour
    {
        [SerializeField] private PauseUIController pauseUI;
        [SerializeField] private Button pauseButton;

        private void OnEnable()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        public void OnPauseClicked()
        {
            pauseUI.Pause();
        }
    }
}