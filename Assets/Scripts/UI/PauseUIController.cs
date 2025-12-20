using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DodoRun.Main;

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

            gameObject.SetActive(false);
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
            gameObject.SetActive(true);

            GameService.Instance.IsGameRunning = false;
            Time.timeScale = 0f;
        }

        private void OnResumeClicked()
        {
            Resume();
        }

        private void OnRestartClicked()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplayScene);
        }

        private void OnHomeClicked()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }

        private void Resume()
        {
            isPaused = false;
            gameObject.SetActive(false);

            Time.timeScale = 1f;
            GameService.Instance.IsGameRunning = true;
        }
    }
}
