namespace DodoRun.Main
{
    public sealed class GameLoop
    {
        private readonly GameService game;

        public GameLoop(GameService game)
        {
            this.game = game;
        }

        public void Tick()
        {
            game.PlatformService.UpdatePlatform();
            game.PlayerService.UpdatePlayer();
            game.CoinService.UpdateCoins();
            game.PowerupService.Update();
            game.ScoreService.Update();
            game.TutorialService.UpdateTutorial();
        }

        public void FixedTick()
        {
            game.PlayerService.FixedUpdatePlayer();
        }
    }
}
