using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DodoRun.Main;
using DodoRun.Sound;

namespace DodoRun.UI
{
    public sealed class PauseUIController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;

        [Header("Scenes")]
        [SerializeField] private string gameplayScene = "Gameplay";
        [SerializeField] private string mainMenuScene = "MainMenu";

        private bool isPaused;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            homeButton.onClick.AddListener(OnHomeClicked);

            if (this.gameObject != null) this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveListener(OnResumeClicked);
            restartButton.onClick.RemoveListener(OnRestartClicked);
            homeButton.onClick.RemoveListener(OnHomeClicked);
        }

        public void Pause()
        {
            if (isPaused) return;

            isPaused = true;

            if (this.gameObject != null)
            {
                this.gameObject.SetActive(true);
                this.gameObject.transform.SetAsLastSibling();
            }

            if (GameService.Instance != null)
            {
                GameService.Instance.IsGameRunning = false;
            }

            Time.timeScale = 0f;
            Debug.Log("Pause UI: Panel should now be visible.");
        }

        public void Resume()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            AudioManager.Instance.SetRunningSoundActive(false);
            isPaused = false;
            if (this.gameObject != null) this.gameObject.SetActive(false); 

            Time.timeScale = 1f;
            GameService.Instance.IsGameRunning = true;
        }

        private void OnResumeClicked() => Resume();

        private void OnRestartClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplayScene);
        }

        private void OnHomeClicked()
        {
            AudioManager.Instance.PlayEffect(SoundType.ButtonClick);
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}