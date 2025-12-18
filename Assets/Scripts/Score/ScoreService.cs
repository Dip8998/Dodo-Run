using UnityEngine;
using TMPro;
using DodoRun.Main;

namespace DodoRun.Score
{
    public class ScoreService
    {
        private float distanceScore;

        private Transform player;

        private TextMeshProUGUI scoreText;
        private TextMeshProUGUI multiplierText;

        private int multiplier = 1;
        private float multiplierTimer = 0f;

        public int TotalScore => Mathf.FloorToInt(distanceScore) + coinScore;
        private int coinScore;
        private int collectedCoins;
        public int CollectedCoins => collectedCoins;

        public void Initialize(TextMeshProUGUI scoreUI, TextMeshProUGUI multiplierUI)
        {
            scoreText = scoreUI;
            multiplierText = multiplierUI;
            Reset();
        }

        public void Update()
        {
            if (!GameService.Instance.IsGameRunning) return;

            if (player == null)
                player = GameService.Instance.PlayerService.GetPlayerTransform();
            if (player == null) return;

            float speed = GameService.Instance.Difficulty.CurrentSpeed;

            float gain = speed * Time.deltaTime * 1.2f;

            if (multiplier > 1)
                gain *= multiplier;

            distanceScore += gain;

            UpdateMultiplierTimer();
            UpdateUI();
        }

        public void AddCoinScore(int amount)
        {
            coinScore += amount;
            UpdateUI();
        }

        public void AddCoins(int amount)
        {
            collectedCoins += amount;
        }

        private void UpdateMultiplierTimer()
        {
            if (multiplier > 1)
            {
                multiplierTimer -= Time.deltaTime;

                if (multiplierTimer <= 0)
                {
                    multiplier = 1;        
                    multiplierTimer = 0;
                }
            }
        }

        public void ActivateDoubleScore()
        {
            multiplier = 2;
            multiplierTimer = 10f; 
            UpdateUI();
        }

        public void DeactivateDoubleScore()
        {
            multiplier = 1;
            multiplierTimer = 0f;
            UpdateUI();
        }

        public void Reset()
        {
            distanceScore = 0;
            multiplier = 1;
            multiplierTimer = 0;
            coinScore = 0;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (scoreText != null)
                scoreText.text = TotalScore.ToString();

            if (multiplierText != null)
                multiplierText.text = $"x{multiplier}";
        }
    }
}
