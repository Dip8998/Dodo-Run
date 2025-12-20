using DodoRun.Data;
using DodoRun.Main;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DodoRun.UI
{
    public sealed class GameOverUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI coinText;

        [SerializeField] private Button homeButton;
        [SerializeField] private Button retryButton;

        [Header("Scenes")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameplayScene = "Gameplay";

        private void Awake()
        {
            homeButton.onClick.AddListener(OnHomeClicked);
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void OnDestroy()
        {
            homeButton.onClick.RemoveListener(OnHomeClicked);
            retryButton.onClick.RemoveListener(OnRetryClicked);
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var scoreService = GameService.Instance.ScoreService;

            scoreText.text = scoreService.TotalScore.ToString();
            coinText.text = scoreService.CollectedCoins.ToString();
        }

        private void OnHomeClicked()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        private void OnRetryClicked()
        {
            SceneManager.LoadScene(gameplayScene);
        }
    }
}
