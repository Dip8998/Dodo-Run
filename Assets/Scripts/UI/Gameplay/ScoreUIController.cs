using TMPro;
using DodoRun.Score;

namespace DodoRun.UI.Controllers
{
    public sealed class ScoreUIController
    {
        private readonly TextMeshProUGUI score;
        private readonly TextMeshProUGUI multiplier;
        private readonly ScoreService service;

        public ScoreUIController(
            TextMeshProUGUI score,
            TextMeshProUGUI multiplier,
            ScoreService service)
        {
            this.score = score;
            this.multiplier = multiplier;
            this.service = service;
            service.Initialize(score, multiplier);
        }

        public void Dispose() { }
    }
}
