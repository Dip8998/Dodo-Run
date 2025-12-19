using UnityEngine;

namespace DodoRun.Main
{
    public sealed class DifficultyManager
    {
        private readonly DifficultySettings settings;
        private readonly float startTime;

        public DifficultyManager(DifficultySettings settings)
        {
            this.settings = settings;
            startTime = Time.time;
        }

        public float Progress =>
            Mathf.Clamp01((Time.time - startTime) / settings.DifficultyRampTime);

        public float CurrentObstacleProbability =>
            Mathf.Lerp(settings.StartObstacleProbability, settings.MaxObstacleProbability, Progress);

        public float CurrentHardObstacleChance =>
            Mathf.Lerp(settings.StartHardObstacleChance, settings.MaxHardObstacleChance, Progress);

        public float CurrentSpeed =>
            Mathf.Lerp(settings.StartSpeed, settings.MaxSpeed, Progress);
    }
}
