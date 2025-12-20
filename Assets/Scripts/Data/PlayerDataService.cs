using UnityEngine;

namespace DodoRun.Data
{
    public static class PlayerDataService
    {
        private const string COIN_KEY = "TOTAL_COINS";
        private const string HIGH_SCORE_KEY = "HIGH_SCORE";
        private const string TUTORIAL_DONE_KEY = "TUTORIAL_DONE";

        public static int TotalCoins
        {
            get => PlayerPrefs.GetInt(COIN_KEY, 0);
            set
            {
                PlayerPrefs.SetInt(COIN_KEY, value);
                PlayerPrefs.Save();
            }
        }

        public static void AddCoins(int amount)
        {
            TotalCoins += amount;
        }

        public static int HighScore
        {
            get => PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            private set
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, value);
                PlayerPrefs.Save();
            }
        }

        public static bool TrySetHighScore(int score)
        {
            if (score <= HighScore)
                return false;

            HighScore = score;
            return true;
        }

        public static bool IsTutorialCompleted
        {
            get => PlayerPrefs.GetInt(TUTORIAL_DONE_KEY, 0) == 1;
        }

        public static void MarkTutorialCompleted()
        {
            PlayerPrefs.SetInt(TUTORIAL_DONE_KEY, 1);
            PlayerPrefs.Save();
        }
    }
}
